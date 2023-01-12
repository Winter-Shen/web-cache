using System.Xml.Serialization;
using System.Collections.Generic;

namespace Server
{
    [XmlRootAttribute("BlockManager", Namespace = "server.BlockManager", IsNullable = false)]
    public class BlockManager
    {
        [XmlArrayAttribute("FileList")]
        public List<CompleteFile> fileList { get; set; }
    }

    [XmlRootAttribute("File")]
    public class CompleteFile
    {
        [XmlArrayAttribute("BlockList")]
        public List<string> blockList { get; set; }
        [XmlAttribute("FileName")]
        public string fileName { get; set; }
        [XmlAttribute("Size")]
        public int size { get; set; }
    }
}
