using System.Xml.Serialization;
using System.Globalization;
using System;

namespace Utils
{
    [XmlRootAttribute("Message", Namespace = "abc.abc", IsNullable = false)]
    public class MessageTCP
    {
        public MessageTCP() { }
        public MessageTCP(MessageType Type, string Content)
        {
            type = Type;
            content = Content;
        }
        [XmlAttribute("Type")]
        public MessageType type { get; set; }
        [XmlElement("Content")]
        public string content { get; set; }
    }


    public enum MessageType { DOWNLOAD, FILELIST, DONE, TEST };
}