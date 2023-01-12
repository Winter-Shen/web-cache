using System;
using System.Xml.Serialization;
using System.Xml;
using System.IO;

namespace Utils
{
    public class SerializationUtil
    {
        public static byte[] objectToBytes(object o, Type type)
        {
            byte[] bytes;
            XmlSerializer xmlSerializer = new XmlSerializer(type);
            using (MemoryStream memoryStream = new MemoryStream())
            {
                xmlSerializer.Serialize(memoryStream, o);
                memoryStream.Seek(0, SeekOrigin.Begin);
                bytes = new byte[memoryStream.Length];
                memoryStream.Read(bytes, 0, Convert.ToInt32(memoryStream.Length));
            }
            return bytes;
        }

        public static object bytesToObject(byte[] bytes, Type type)
        {
            XmlSerializer xmlSerializer = new XmlSerializer(type);
            object o;
            using (Stream stream = new MemoryStream(bytes))
            {
                o = xmlSerializer.Deserialize(stream);
            }
            return o;
        }

        public static void saveXml(object o, string filePath, Type type)
        {
            using (StreamWriter writer = new StreamWriter(filePath))
            {
                XmlSerializer xmlSerializer = new XmlSerializer(type);
                xmlSerializer.Serialize(writer, o);
            }
        }

        public static object loadXml(string filePath, Type type)
        {
            object o;
            using (StreamReader reader = new StreamReader(filePath))
            {
                XmlSerializer xmlSerializer = new XmlSerializer(type);
                o = xmlSerializer.Deserialize(reader);
            }
            return o;
        }

        //public static byte[] objectToBytes_NotForConnect(Message message)
        //{
        //    byte[] bytes;
        //    XmlSerializer xmlSerializer = new XmlSerializer(message.GetType());
        //    using (MemoryStream memoryStream = new MemoryStream())
        //    {
        //        xmlSerializer.Serialize(memoryStream, message);
        //        memoryStream.Seek(0, SeekOrigin.Begin);
        //        bytes = new byte[memoryStream.Length];
        //        memoryStream.Read(bytes, 0, bytes.Length);
        //    }
        //    return bytes;
        //}

        //public static byte[] loadXml_NotForConnect(string filePath)
        //{
        //    Message message;
        //    using (StreamReader reader = new StreamReader(filePath))
        //    {
        //        XmlSerializer xmlSerializer = new XmlSerializer(typeof(Message));
        //        message = (Message)xmlSerializer.Deserialize(reader);
        //    }
        //    return objectToBytes_NotForConnect(message);
        //}







        //public static void saveXml(byte[] bytes, string filePath)
        //{
        //    Message message = bytesToObject(bytes);
        //    using (StreamWriter writer = new StreamWriter(filePath))
        //    {
        //        XmlSerializer xmlSerializer = new XmlSerializer(typeof(Message));
        //        xmlSerializer.Serialize(writer, message);
        //    }
        //}

        //public static byte[] loadXml(string filePath)
        //{
        //    Message message;
        //    using (StreamReader reader = new StreamReader(filePath))
        //    {
        //        XmlSerializer xmlSerializer = new XmlSerializer(typeof(Message));
        //        message = (Message)xmlSerializer.Deserialize(reader);
        //    }
        //    return objectToBytes(message);
        //}
    }
}
