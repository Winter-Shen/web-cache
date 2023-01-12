using System.Net;
using Utils;
namespace Client
{
    public partial class Form1 : Form
    {
        string[] files;
        IPHostEntry ipHost;
        IPAddress serverIpAddr;
        int cachePort = 11111;
        public Form1()
        {
            InitializeComponent();
            ipHost = Dns.GetHostEntry(Dns.GetHostName());
            serverIpAddr = ipHost.AddressList[0];
            cachePort = 11111;
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            ClientConnect clientEdge = new ClientConnect(serverIpAddr, cachePort);
            string[] files = (string[])clientEdge.clienEdge(new Object[] { MessageType.FILELIST });

            this.listBox2.Items.Clear();
            if(files != null)
            {
                this.listBox2.Items.AddRange(files);
            }

            this.listBox1.Items.Clear();
            ClientFile clientFile = new ClientFile();
            DirectoryInfo dir = new DirectoryInfo(clientFile.filesDirectory);
            FileInfo[] fileInfos = dir.GetFiles();
            foreach(FileInfo fileInfo in fileInfos)
            {
                this.listBox1.Items.Add(fileInfo.Name);
            }
        }

        private void listBox1_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {

        }

        private void textBox2_TextChanged(object sender, EventArgs e)
        {

        }

        private void refresh(object sender, EventArgs e)
        {
            ClientConnect clientEdge = new ClientConnect(serverIpAddr, cachePort);
            files = (string[])clientEdge.clienEdge(new Object[] { MessageType.FILELIST});

            this.listBox2.Items.Clear();
            this.listBox2.Items.AddRange(files);
        }

        private void downlaod(object sender, EventArgs e)
        {
            if (this.listBox2.SelectedItems.Count > 0)
            {
                string file = this.listBox2.SelectedItem.ToString();
                ClientConnect clientEdge = new ClientConnect(serverIpAddr, cachePort);
                clientEdge.clienEdge(new Object[] { MessageType.DOWNLOAD, file });
            }

            this.listBox1.Items.Clear();
            ClientFile clientFile = new ClientFile();
            DirectoryInfo dir = new DirectoryInfo(clientFile.filesDirectory);
            FileInfo[] fileInfos = dir.GetFiles();
            foreach (FileInfo fileInfo in fileInfos)
            {
                this.listBox1.Items.Add(fileInfo.Name);
            }
        }

        private void button3_Click(object sender, EventArgs e)
        {
            ClientFile clientFile = new ClientFile();
            if (this.listBox1.SelectedItems.Count > 0)
            {
                string file = this.listBox1.SelectedItem.ToString();
                File.Delete(Path.Combine(clientFile.filesDirectory, file));
            }
            this.listBox1.Items.Clear();
            DirectoryInfo dir = new DirectoryInfo(clientFile.filesDirectory);
            FileInfo[] fileInfos = dir.GetFiles();
            foreach (FileInfo fileInfo in fileInfos)
            {
                this.listBox1.Items.Add(fileInfo.Name);
            }

        }
    }
}