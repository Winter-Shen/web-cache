using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Utils;
using System.IO;

namespace Client
{
    class ClientFile : FileUtil
    {
        public string filesDirectory;
        public string cachedBlocksDirectory;
        public ClientFile()
        {
            rootPath = Directory.GetCurrentDirectory();
            filesDirectory = Path.Combine(rootPath, "Files");
            cachedBlocksDirectory = Path.Combine(rootPath, "CachedBlocks");
            if (!Directory.Exists(filesDirectory))
            {
                Directory.CreateDirectory(filesDirectory);
            }
        }
    }
}
