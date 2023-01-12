using System.Net;
using System.Text;
using Utils;
namespace Cache
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();

        }

        private void Form1_Load(object sender, EventArgs e)
        {
            this.listBox2.Items.Clear();
            CacheFile cacheFile = new CacheFile();
            DirectoryInfo dir = new DirectoryInfo(cacheFile.cachedFileDirectory);
            FileInfo[] fileInfos = dir.GetFiles();
            foreach (FileInfo fileInfo in fileInfos)
            {
                this.listBox2.Items.Add(fileInfo.Name);
            }

            this.listBox1.Items.Clear();
            using (StreamReader sr = new StreamReader(Path.Combine(cacheFile.rootPath, "log")))
            {
                string line;
                while ((line = sr.ReadLine()) != null)
                {
                    this.listBox1.Items.Add(line);
                }
            }

            Thread thread = new Thread(tcp);
            thread.IsBackground = true;
            thread.Start();
        }

        private void tcp()
        {
            IPHostEntry ipHost = Dns.GetHostEntry(Dns.GetHostName());
            IPAddress localIpAddr = ipHost.AddressList[0];
            CacheConnect cacheConnect = new CacheConnect(11111, localIpAddr, 22222);

            new CacheFile();

            cacheConnect.serverEdge(new object[] { });
        }

        private void showDetails(object sender, EventArgs e)
        {
            this.richTextBox1.Clear();
            if (this.listBox2.SelectedItems.Count > 0)
            {
                string file = this.listBox2.SelectedItem.ToString();
                CacheFile cacheFile = new CacheFile();
                byte[] buffer = cacheFile.loadFile(cacheFile.cachedFileDirectory, file);

                StringBuilder sb = new StringBuilder(32);
                for (int j = 0; j < buffer.Length; j++)
                {
                    sb.Append(buffer[j].ToString("X2"));
                }
                string text = sb.ToString();
                this.richTextBox1.AppendText(text);
            }
        }

        private void label1_Click(object sender, EventArgs e)
        {

        }

        private void refresh_log(object sender, EventArgs e)
        {
            CacheFile cacheFile = new CacheFile();
            this.listBox1.Items.Clear();
            using (StreamReader sr = new StreamReader(Path.Combine(cacheFile.rootPath, "log")))
            {
                string line;
                while ((line = sr.ReadLine()) != null)
                {
                    this.listBox1.Items.Add(line);
                }
            }
        }

        private void refresh_blocks(object sender, EventArgs e)
        {
            this.listBox2.Items.Clear();
            CacheFile cacheFile = new CacheFile();
            DirectoryInfo dir = new DirectoryInfo(cacheFile.cachedFileDirectory);
            FileInfo[] fileInfos = dir.GetFiles();
            foreach (FileInfo fileInfo in fileInfos)
            {
                this.listBox2.Items.Add(fileInfo.Name);
            }
        }

        private void button3_Click(object sender, EventArgs e)
        {
            CacheFile cacheFile = new CacheFile();

            //DirectoryInfo di = new DirectoryInfo(cacheFile.cachedFileDirectory);
            //FileInfo[] filePathList = di.GetFiles();
            //string[] fileNameList = new string[filePathList.Length];
            //for (int i = 0; i < filePathList.Length; i++)
            //{
            //    fileNameList[i] = filePathList[i].Name;
            //}


            foreach (string fingerprint in cacheFile.fingerprintManager)
            {
                File.Delete(Path.Combine(cacheFile.cachedFileDirectory, fingerprint));
            }
            cacheFile.fingerprintManager = new List<string>();
            cacheFile.fingerprints = new System.Collections.Hashtable();
            SerializationUtil.saveXml(cacheFile.fingerprintManager, Path.Combine(cacheFile.rootPath, "BlockManager.xml"), typeof(List<string>));
            this.listBox2.Items.Clear();
        }
    }
}