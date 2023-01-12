using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Collections;
using Utils;

namespace Cache
{
    class CacheFile : FileUtil
    {
        public string cachedFileDirectory;
        public List<string> fingerprintManager;
        public Hashtable fingerprints = new Hashtable();
        public CacheFile()
        {
            rootPath = Directory.GetCurrentDirectory();
            cachedFileDirectory = Path.Combine(rootPath, "CachedFile");
            string fingerprintManagerPath = Path.Combine(rootPath, "BlockManager.xml");
            if (!Directory.Exists(cachedFileDirectory))
            {
                Directory.CreateDirectory(cachedFileDirectory);
            }

            if (File.Exists(fingerprintManagerPath))
            {
                fingerprintManager = (List<string>)SerializationUtil.loadXml(fingerprintManagerPath, typeof(List<string>));
            }
            else
            {
                fingerprintManager = new List<string>();
                SerializationUtil.saveXml(fingerprintManager, fingerprintManagerPath, typeof(List<string>));
            }

            for(int i = 0; i < fingerprintManager.Count; i++)
            {
                fingerprints.Add(fingerprintManager[i], i);
;           }
            FileStream fs = new FileStream(Path.Combine(rootPath, "log"), FileMode.OpenOrCreate);
            fs.Close();
        }
    }
}
