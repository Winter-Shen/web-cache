using System;
using System.IO;
using System.Text;
using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography;
using Utils;

namespace Server
{

    class ServerFile : FileUtil
    {
        
        public string fileDirectory;
        public string blockDirectory;
        public int blockSize = 2048;
        public Hashtable fingerprint = new Hashtable(); // store md5 values
        public Hashtable fileTable = new Hashtable(); // store file names
        public BlockManager blockManager = new BlockManager(); // map files and blocks(md5 value)

        public ServerFile()
        {
            rootPath = Directory.GetCurrentDirectory();
            fileDirectory = Path.Combine(rootPath, "files");
            blockDirectory = Path.Combine(rootPath, "blocks");
            // xml file to store map of blocks and blocks(md5 value)
            string blockManagerPath = Path.Combine(rootPath, "BlockManager.xml"); 
            // if no "BlockManager.xml", create it
            if (File.Exists(blockManagerPath))
            {
                blockManager = (BlockManager)SerializationUtil.loadXml(blockManagerPath, typeof(BlockManager));
            }
            else
            {
                blockManager.fileList = new List<CompleteFile>();
                SerializationUtil.saveXml(blockManager, Path.Combine(rootPath, "BlockManager.xml"), typeof(BlockManager));
            }
            if (!Directory.Exists(fileDirectory))
            {
                Directory.CreateDirectory(fileDirectory);
            }
            if (!Directory.Exists(blockDirectory))
            {
                Directory.CreateDirectory(blockDirectory);
            }
            // initial `fingerprint` and `fileTable` from `blockManager`
            for (int i = 0; i < blockManager.fileList.Count; i++)
            {
                CompleteFile file = blockManager.fileList[i];
                fileTable.Add(file.fileName, i);
                foreach(string md5String in file.blockList)
                {
                    if (!fingerprint.ContainsKey(md5String))
                    {
                        fingerprint.Add(md5String, null);
                    }
                }
            }
        }

        // create blocks, fingerprints and BlockManager.xml
        public void initial()
        {
            string[] fileNameList = getFileName(fileDirectory);
            foreach(string fileName in fileNameList)
            {
                if (!fileTable.ContainsKey(fileName))
                {
                    partion(fileName);
                }
            }
            
        }
        // partion a file into blocks
        public void partion(string fileName)
        {
            CompleteFile blockOfFile = new CompleteFile();
            blockOfFile.fileName = fileName;
            blockOfFile.blockList = new List<string>();
            fileTable.Add(fileName, blockManager.fileList.Count);
            byte[] bytes = loadFile(fileDirectory, fileName);
            blockOfFile.size = bytes.Length;
            int fileSize = bytes.Length;
            int blockNum = fileSize / blockSize;
            int lastBlockSize = fileSize % blockSize;
            int _blockSize = blockSize;

            for (int i = 0, p = 0; i < blockNum + 1; i++, p = p + _blockSize)
            {
                if(i == blockNum)
                {
                    _blockSize = lastBlockSize;
                }

                // block in byte
                byte[] blockBytes = new Byte[_blockSize];
                Buffer.BlockCopy(bytes, p, blockBytes, 0, _blockSize);

                // generate md5
                MD5CryptoServiceProvider md5 = new MD5CryptoServiceProvider();
                md5.ComputeHash(blockBytes);

                // md5 in byte
                byte[] md5Byte = md5.Hash;
                md5.Clear();

                // md5 in string
                StringBuilder sb = new StringBuilder(32);
                for (int j = 0; j < md5Byte.Length; j++)
                {
                    sb.Append(md5Byte[j].ToString("X2"));
                }
                string MD5String = sb.ToString();

                // save block
                //   save block in `blockManger`
                blockOfFile.blockList.Add(MD5String);
                //   save block in file and name it with md5 string, if exist.
                if (!fingerprint.ContainsKey(MD5String))
                {
                    fingerprint.Add(MD5String, null);
                    saveFile(blockBytes, blockDirectory, MD5String);
                }
            }
            blockManager.fileList.Add(blockOfFile);
            SerializationUtil.saveXml(blockManager, Path.Combine(rootPath, "BlockManager.xml"), typeof(BlockManager));
        }
    }
}
