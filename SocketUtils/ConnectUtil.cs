using System;
using System.Net;
using System.Net.Sockets;

namespace Utils
{

    public class ConnectUtil
    {
        public IPAddress localIpAddr;
        public IPEndPoint listenEndPoint;

        public IPAddress serverIpAddr;
        public IPEndPoint serverEndPoint;

        public ConnectUtil()
        {
            IPHostEntry ipHost = Dns.GetHostEntry(Dns.GetHostName());
            localIpAddr = ipHost.AddressList[0];
        }

        public ConnectUtil(int ListenPort)
        {
            IPHostEntry ipHost = Dns.GetHostEntry(Dns.GetHostName());
            localIpAddr = ipHost.AddressList[0];

            listenEndPoint = new IPEndPoint(localIpAddr, ListenPort);
        }

        public ConnectUtil(IPAddress ServerIpAddr, int ServerPort)
        {
            IPHostEntry ipHost = Dns.GetHostEntry(Dns.GetHostName());
            localIpAddr = ipHost.AddressList[0];

            serverIpAddr = ServerIpAddr;
            serverEndPoint = new IPEndPoint(ServerIpAddr, ServerPort);
        }

        public ConnectUtil(int ListenPort, IPAddress ServerIpAddr, int ServerPort)
        {
            IPHostEntry ipHost = Dns.GetHostEntry(Dns.GetHostName());
            localIpAddr = ipHost.AddressList[0];

            listenEndPoint = new IPEndPoint(localIpAddr, ListenPort);

            serverIpAddr = ServerIpAddr;
            serverEndPoint = new IPEndPoint(ServerIpAddr, ServerPort);
        }

        // Client call the function to establish TCP connect and send and receive message
        public object clienEdge(object[] args)
        {
            try
            {
                // Creation TCP/IP Socket
                Socket sender = new Socket(localIpAddr.AddressFamily, //IPv4 or IPv6
                           SocketType.Stream, ProtocolType.Tcp);
                try
                {
                    // Connect Socket to the remote endpoint
                    sender.Connect(serverEndPoint);

                    // communicate with server
                    object result = clientCommunication(sender,args);


                    // Close Socket
                    sender.Shutdown(SocketShutdown.Both);
                    sender.Close();
                    return result;
                }

                // Manage of Socket's Exceptions
                catch (ArgumentNullException ane)
                {

                    Console.WriteLine("ArgumentNullException : {0}", ane.ToString());
                }

                catch (SocketException se)
                {

                    Console.WriteLine("SocketException : {0}", se.ToString());
                }

                catch (Exception e)
                {
                    Console.WriteLine("Unexpected exception : {0}", e.ToString());
                }
            }

            catch (Exception e)
            {

                Console.WriteLine(e.ToString());
            }
            return null;
        }

        // Server call the function to listen and receive and send message
        public object serverEdge(object[] args)
        {
            // Creation TCP/IP Socket 
            Socket listener = new Socket(localIpAddr.AddressFamily, SocketType.Stream, ProtocolType.Tcp);

            try
            {

                // Associate a network address to the Server Socket
                listener.Bind(listenEndPoint);

                // Create the Client list that will want to connect to Server
                listener.Listen(10);

                while (true)
                {

                    Console.WriteLine("Waiting connection ... ");

                    // Accept connection of client
                    Socket clientSocket = listener.Accept();

                    // Communicate with client
                    object result = serverCommunication(clientSocket, args);

                    // Close client Socket 
                    clientSocket.Shutdown(SocketShutdown.Both);
                    clientSocket.Close(); 
                }
            }

            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
            }
            return null;
        }

        public virtual object clientCommunication(Socket socket, object[] args)
        {
            // Generate bytes
            MessageTCP message = new MessageTCP(MessageType.TEST, null);
            byte[] bytes = generateBytes(message, null, null);

            // Send bytes
            int byteSent = socket.Send(bytes);

            // Receive size of Message object
            byte[] messageLnegth = new Byte[4];
            socket.Receive(messageLnegth);
            int length = BitConverter.ToInt32(messageLnegth, 0);

            // Receive Message object
            byte[] messageBytes = new Byte[length];
            socket.Receive(messageBytes);
            message = (MessageTCP)SerializationUtil.bytesToObject(messageBytes, typeof(MessageTCP));

            return message;
        }

        public virtual object serverCommunication(Socket socket, object[] args)
        {
            // Receive size of Message object
            byte[] messageLnegth = new Byte[4];
            socket.Receive(messageLnegth);
            int length = BitConverter.ToInt32(messageLnegth, 0);

            // Receive Message object
            byte[] messageBytes = new Byte[length];
            socket.Receive(messageBytes);
            MessageTCP message = (MessageTCP)SerializationUtil.bytesToObject(messageBytes, typeof(MessageTCP));

            // Generate bytes
            byte[] bytes = generateBytes(message, null, null);

            // Send a message to Client
            socket.Send(bytes);

            return null;
        }

        public byte[] generateBytes(MessageTCP message, object file, Type fileType)
        {
            byte[] bytes, messageByte, messageLength;
            if (file != null)
            {
                byte[] fileByte = SerializationUtil.objectToBytes(file, fileType);
                
                messageByte = SerializationUtil.objectToBytes(message, typeof(MessageTCP));
                messageLength = BitConverter.GetBytes(messageByte.Length);

                bytes = new Byte[messageByte.Length + fileByte.Length + 4];
                Buffer.BlockCopy(messageLength, 0, bytes, 0, 4);
                Buffer.BlockCopy(messageByte, 0, bytes, 4, messageByte.Length);
                Buffer.BlockCopy(fileByte, 0, bytes, 4 + messageByte.Length, fileByte.Length);
            }
            else
            {
                messageByte = SerializationUtil.objectToBytes(message, typeof(MessageTCP));
                messageLength = BitConverter.GetBytes(messageByte.Length);

                bytes = new Byte[messageByte.Length + 4];
                Buffer.BlockCopy(messageLength, 0, bytes, 0, 4);
                Buffer.BlockCopy(messageByte, 0, bytes, 4, messageByte.Length);
            }
            return bytes;
        }

        public void SendBytes(Socket socket, byte[] contentBytes)
        {
            byte[] bytes = new Byte[contentBytes.Length + 4];
            byte[] lengthBytes = BitConverter.GetBytes(contentBytes.Length);
            Buffer.BlockCopy(lengthBytes, 0, bytes, 0, 4);
            Buffer.BlockCopy(contentBytes, 0, bytes, 4, contentBytes.Length);
            socket.Send(bytes);
        }

        public int receiveBytes(Socket socket, out byte[] contentBytes)
        {
            byte[] lengthBytes = new Byte[4];
            socket.Receive(lengthBytes);
            int length = BitConverter.ToInt32(lengthBytes, 0);
            contentBytes = new Byte[length];
            socket.Receive(contentBytes);
            return length;
        }
    }
}
