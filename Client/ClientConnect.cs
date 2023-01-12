using System.Net;
using System.Net.Sockets;
using System;
using System.Collections.Generic;
using Utils;

namespace Client
{
    class ClientConnect : ConnectUtil
    {
        public ClientConnect(IPAddress ServerIpAddr, int ServerPort) : base(ServerIpAddr, ServerPort) { }

        public override object clientCommunication(Socket socket, object[] args)
        {
            byte[] bytes, contentBytes, lengthBytes;
            int length;
            switch (Convert.ToInt32((MessageType)args[0]))
            {
                case (int)MessageType.FILELIST:
                    return FILELIST(socket);
                case (int)MessageType.DOWNLOAD:
                    DOWNLOAD(socket, (string)args[1]);
                    break;
            }

            return null;
        }

        public object FILELIST(Socket socket)
        {
            byte[] bytes, contentBytes, lengthBytes;
            int length;
            MessageTCP message = new MessageTCP(MessageType.FILELIST, null);
            contentBytes = SerializationUtil.objectToBytes(message, typeof(MessageTCP));
            SendBytes(socket, contentBytes);
            receiveBytes(socket, out contentBytes);
            string[] fileNameList = (string[])SerializationUtil.bytesToObject(contentBytes, typeof(string[]));
            return fileNameList;
        }

        public object DOWNLOAD(Socket socket, string fileName)
        {
            byte[] contentBytes, lengthBytes;
            int length;
            MessageTCP message = new MessageTCP(MessageType.DOWNLOAD, fileName);
            // Request file
            contentBytes = SerializationUtil.objectToBytes(message, typeof(MessageTCP));
            SendBytes(socket, contentBytes);

            // Receive size of file 
            receiveBytes(socket, out contentBytes);
            int size = (int)SerializationUtil.bytesToObject(contentBytes, typeof(int));

            // Receive length of md5 list
            contentBytes = new byte[4];
            socket.Receive(contentBytes);
            int listLength = BitConverter.ToInt32(contentBytes);

            // Receive md5List
            List<string> md5List = new List<string>();
            for (int i = 0; i < listLength; i++)
            {
                receiveBytes(socket, out contentBytes);
                md5List.Add((string)SerializationUtil.bytesToObject(contentBytes, typeof(string)));
            }
            
            // Receive blocks
            byte[] bytes = new byte[size];
            for (int i = 0, p = 0; i < md5List.Count; i++, p = p + contentBytes.Length)
            {
                receiveBytes(socket, out contentBytes);
                Buffer.BlockCopy(contentBytes, 0, bytes, p, contentBytes.Length);
            }

            // Save as file
            ClientFile clientFile = new ClientFile();
            clientFile.saveFile(bytes, clientFile.filesDirectory, fileName);

            return null;
        }
        
    }
}
