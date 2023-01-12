using Utils;
using System.Net;
using System.Text;
using System.Net.Sockets;
using System.IO;
using System.Collections.Generic;

namespace Cache
{
    class CacheConnect : ConnectUtil
    {
        float rate = 0;
        public CacheConnect(int ListenPort) : base(ListenPort) { }

        public CacheConnect(int ListenPort, IPAddress ServerIpAddr, int ServerPort) : base(ListenPort, ServerIpAddr, ServerPort) { }

        public override object serverCommunication(Socket socket, object[] args)
        {
            byte[] contentBytes;
            receiveBytes(socket, out contentBytes);
            MessageTCP message = (MessageTCP)SerializationUtil.bytesToObject(contentBytes, typeof(MessageTCP));
            switch (Convert.ToInt32(message.type))
            {
                case (int)MessageType.FILELIST:
                    FILELIST_IN(message, socket);
                    break;
                case (int)MessageType.DOWNLOAD:
                    DOWNLOAD_IN(message, socket);
                    break;
            }
            return null;
        }
       
        public void FILELIST_IN(MessageTCP message, Socket socket)
        {
            byte[] contentBytes = (byte[])clienEdge(new Object[] { message });
            SendBytes(socket, contentBytes);
        }

        public object DOWNLOAD_IN(MessageTCP message, Socket socket)
        {
            byte[] contentBytes;
            CacheFile cacheFile = new CacheFile();
            List<string> md5List =  (List<string>)clienEdge(new Object[] { message, socket });

            for(int i = 0; i < md5List.Count; i++)
            {
                contentBytes = cacheFile.loadFile(cacheFile.cachedFileDirectory, md5List[i]);
                SendBytes(socket, contentBytes);
            }

            StringBuilder sb = new StringBuilder();
            DateTime dateTime = DateTime.Now;
            sb.AppendLine("user request: file " + message.content + " " + dateTime.ToString());
            sb.Append("response: " + rate + "% of file " + message.content + " was constructed with the cached data");
            cacheFile.write(Path.Combine(cacheFile.rootPath, "log"), sb.ToString());
            return null;

        }

        public override object clientCommunication(Socket socket, object[] args)
        {
            MessageTCP message = (MessageTCP)args[0];

            switch (Convert.ToInt32(message.type))
            {
                case (int)MessageType.FILELIST:
                    return FILELIST_OUT(message, socket);
                case (int)MessageType.DOWNLOAD:
                    return DOWNLOAD_OUT(message, socket, (Socket)args[1]);
            }
            return null;
        }

        public object FILELIST_OUT(MessageTCP message, Socket socket)
        {
            byte[] contentBytes;
            contentBytes = SerializationUtil.objectToBytes(message, typeof(MessageTCP));
            SendBytes(socket, contentBytes);
            receiveBytes(socket, out contentBytes);
            return contentBytes;
        }

        public object DOWNLOAD_OUT(MessageTCP message, Socket socket,Socket clientSocket)
        {
            byte[] contentBytes;
            string fileName = message.content;
            CacheFile cacheFile = new CacheFile();
            // Send file name
            contentBytes = SerializationUtil.objectToBytes(message, typeof(MessageTCP));
            SendBytes(socket, contentBytes);

            // Receive size of file
            receiveBytes(socket, out contentBytes);
            int size = (int)SerializationUtil.bytesToObject(contentBytes, typeof(int));

            // Send size of file to client
            SendBytes(clientSocket, contentBytes);

            // Receive the length of md5 list 
            contentBytes = new byte[4];
            socket.Receive(contentBytes);
            int listLength = BitConverter.ToInt32(contentBytes);

            // Send the length of md5 list to client
            clientSocket.Send(contentBytes);

            // Receive md5List
            List<string> md5List = new List<string>();
            for (int i = 0; i < listLength; i++)
            {
                receiveBytes(socket, out contentBytes);
                md5List.Add((string)SerializationUtil.bytesToObject(contentBytes, typeof (string)));
                // Send md5 list to client
                SendBytes(clientSocket, contentBytes);
            }

            // Select md5 of uncached block
            List<string> md5ListNotCached = new List<string>();
            foreach(string md5 in md5List)
            {
                if (!cacheFile.fingerprints.ContainsKey(md5))
                {
                    md5ListNotCached.Add(md5);
                }
            }

            rate = (md5List.Count - md5ListNotCached.Count);
            rate = rate / md5List.Count;
            rate = rate * 100;

            // Send length of md5 list of uncached blocks
            socket.Send(BitConverter.GetBytes(md5ListNotCached.Count));

            // Send md5 list of uncached blocks
            for (int i = 0; i < md5ListNotCached.Count; i++)
            {
                contentBytes = SerializationUtil.objectToBytes(md5ListNotCached[i], typeof(string));
                SendBytes(socket, contentBytes);
            }

            // Receive and store blocks
            for(int i = 0; i < md5ListNotCached.Count; i++)
            {
                receiveBytes(socket, out contentBytes);
                cacheFile.saveFile(contentBytes, cacheFile.cachedFileDirectory, md5ListNotCached[i]);
                cacheFile.fingerprints.Add(md5ListNotCached[i], cacheFile.fingerprintManager.Count);
                cacheFile.fingerprintManager.Add(md5ListNotCached[i]);
            }

            SerializationUtil.saveXml(cacheFile.fingerprintManager, Path.Combine(cacheFile.rootPath, "BlockManager.xml"), typeof(List<string>));
            return md5List;
        }
    }
}
