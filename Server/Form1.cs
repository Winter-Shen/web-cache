using System.Net;
using System.Text;
namespace Server
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void refresh_file_directory(object sender, EventArgs e)
        {
            ServerFile serverFile = new ServerFile();
            serverFile.initial();
        }

        private void tcp()
        {
            ServerFile serverFile = new ServerFile();
            serverFile.initial();
            ServerConnect serverConnect = new ServerConnect(22222);
            serverConnect.serverEdge(new object[] { });
        }

        private void Form1_Load_1(object sender, EventArgs e)
        {
            Thread thread = new Thread(tcp);
            thread.IsBackground = true;
            thread.Start();
        }

        private void clear_file_directory(object sender, EventArgs e)
        {
            ServerFile serverFile = new ServerFile();

            DirectoryInfo root = new DirectoryInfo(serverFile.blockDirectory);
            FileInfo[] files = root.GetFiles();

            foreach (FileInfo file in files)
            {
                File.Delete(file.FullName);
            }

            File.Delete(Path.Combine(serverFile.rootPath, "BlockManager.xml"));
        }
    }
}