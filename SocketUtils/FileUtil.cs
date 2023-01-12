using System;
using System.IO;
using System.Security.Cryptography;

using System.Text;

namespace Utils
{
    public class FileUtil
    {
        public string rootPath;
        public string[] getFileName(string directory)
        {
            DirectoryInfo di = new DirectoryInfo(directory);
            FileInfo[] filePathList = di.GetFiles();
            string[] fileNameList = new string[filePathList.Length];
            for (int i = 0; i < filePathList.Length; i++)
            {
                fileNameList[i] = filePathList[i].Name;
            }
            return fileNameList;
        }

        public byte[] loadFile(string directory, string fileName)
        {
            string filePath = Path.Combine(directory, fileName);
            byte[] bytes;
            using (FileStream stream = new FileInfo(filePath).OpenRead())
            {
                bytes = new byte[stream.Length];
                stream.Read(bytes, 0, Convert.ToInt32(stream.Length));
            }
            return bytes;
        }

        public void saveFile(byte[] bytes, string directory, string fileName)
        {
            string filePath = Path.Combine(directory, fileName);
            using (FileStream fstream = File.Create(filePath, bytes.Length))
            {
                fstream.Write(bytes, 0, bytes.Length);
            }
        }

        public string computeMd5Value(byte[] bytes)
        {
            // generate md5
            MD5CryptoServiceProvider md5 = new MD5CryptoServiceProvider();
            md5.ComputeHash(bytes);

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
            return MD5String;
        }

        public void write(string filePath, string content)
        {
            FileStream fs = new FileStream(filePath, FileMode.Append);
            StreamWriter sw = new StreamWriter(fs);
            sw.WriteLine(content);
            sw.Close();
            fs.Close();
        }
    }
}
