using System.Net.Sockets;
using System;
using System.Collections.Generic;
using Utils;
using System.IO;

namespace Server
{
    class ServerConnect : ConnectUtil
    {
        public ServerConnect(int ListenPort) : base(ListenPort) { }

        public override object serverCommunication(Socket socket, object[] args)
        {
            byte[] bytes, contentBytes, lengthBytes;
            int length;
            receiveBytes(socket, out contentBytes);
            MessageTCP message = (MessageTCP)SerializationUtil.bytesToObject(contentBytes, typeof(MessageTCP));
            switch (Convert.ToInt32(message.type))
            {
                case (int)MessageType.FILELIST:
                    FILELIST(socket);
                    break;
                case (int)MessageType.DOWNLOAD:
                    DOWNLOAD(socket, message);
                    break;
            }
            return null;
        }

        public void FILELIST(Socket socket)
        {
            ServerFile serverFile = new ServerFile();
            MessageTCP message = new MessageTCP();

            byte[] bytes, contentBytes, lengthBytes;
            int length;

            string[] fileNameList = new string[serverFile.blockManager.fileList.Count];
            for(int i = 0; i < serverFile.blockManager.fileList.Count; i++)
            {
                fileNameList[i] = serverFile.blockManager.fileList[i].fileName;
            }
            contentBytes = SerializationUtil.objectToBytes(fileNameList, typeof(string[]));
            SendBytes(socket, contentBytes);

        }
        public void DOWNLOAD(Socket socket, MessageTCP message)
        {
            byte[] bytes, contentBytes, lengthBytes;
            int length;

            // Get the blocks of file
            ServerFile serverFile = new ServerFile();
            int idx = (int)serverFile.fileTable[message.content];
            List<string> md5List = serverFile.blockManager.fileList[idx].blockList;
            int size = serverFile.blockManager.fileList[idx].size;

            // Send size of file
            contentBytes = SerializationUtil.objectToBytes(size, typeof(int));
            SendBytes(socket, contentBytes);

            // Send length of md5 list
            socket.Send(BitConverter.GetBytes(md5List.Count));
            // Send md5List
            for(int i = 0; i < md5List.Count; i++)
            {
                contentBytes = SerializationUtil.objectToBytes(md5List[i], typeof(string));
                SendBytes(socket, contentBytes);
            }

            // Receive length of md5 list of uncached blocks
            contentBytes = new byte[4];
            socket.Receive(contentBytes);
            int listLength = BitConverter.ToInt32(contentBytes);

            // Receive md5 list of uncached blocks
            List<string> md5ListNotCache = new List<string>();
            for (int i = 0; i < listLength; i++)
            {
                receiveBytes(socket, out contentBytes);
                md5ListNotCache.Add((string)SerializationUtil.bytesToObject(contentBytes, typeof(string)));
            }

            // Send blocks
            for(int i = 0; i < md5ListNotCache.Count; i++)
            {
                contentBytes = serverFile.loadFile(serverFile.blockDirectory, md5List[i]);
                SendBytes(socket, contentBytes);
            }
        }
    }
}