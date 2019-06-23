using System;
using System.ComponentModel;
using System.Windows.Forms;
using System.Net;
using System.IO;
using System.IO.Compression;
using System.Threading;

namespace PatchDLL
{
    public partial class Form1 : Form
    {
        // The URL for patching
        // By default patches are read in the format VERSION_to_VERSION+1.zip
        // Example: http://cdn.mabiclassic.com/patch/154_to_155.zip
        const string patchURL = "http://cdn.mabiclassic.com/patch/";

        // The URL holding patch information
        const string patchInfoURL = "http://cdn.mabiclassic.com/patch/patch.txt";

        // The file name that the local client version is stored in
        // By default this file is stored in the same directory
        const string clientVerFile = "mabiver.ini";

        // Holds the arguments for launching client.exe
        const string launchArgs = " code:1622 ver:126 logip:142.44.223.136 logport:11000 chatip:142.44.223.136 chatport:8002 setting:\"file://data/features.xml=Regular, USA\"";

        // Argument to look for when checking remote client version
        const string patchInfoArg = "main_version";

        // Default server version
        int mainVer = 125;

        // Default client version
        int mainVerClient = 125;

        public Form1()
        {
            InitializeComponent();
        }

        // All patching setup done here
        private void Form1_Load(object sender, EventArgs e)
        {
            // Defining variables
            string html = string.Empty;
            StreamReader mabiver = new StreamReader(clientVerFile);
            mainVerClient = int.Parse(mabiver.ReadLine());
            mabiver.Close();

            // Grab patch.txt and assign values
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(patchInfoURL);
            using (HttpWebResponse response = (HttpWebResponse)request.GetResponse())
            using (Stream stream = response.GetResponseStream())
            using (StreamReader reader = new StreamReader(stream))
            {
                while (reader.Peek() >= 0)
                {
                    html = reader.ReadLine();
                    if (html[0] == patchInfoArg[0] && html[1] == patchInfoArg[1] && html[2] == patchInfoArg[2] && html[3] == patchInfoArg[3] && html[4] == patchInfoArg[4] && html[5] == patchInfoArg[5])
                        mainVer = int.Parse(html.Remove(0, 13));
                }
            }

            // Download Files
            if (mainVerClient < mainVer)
            {
                textBox1.Text = "Updating " + Convert.ToString(mainVerClient) + " to " + Convert.ToString(mainVerClient + 1);
                try
                {
                    using (var newPatch = new WebClient())
                    {
                        newPatch.DownloadFileCompleted += wc_DownloadCompleted;
                        newPatch.DownloadProgressChanged += wc_DownloadProgressChanged;
                        newPatch.DownloadFileAsync(new Uri(patchURL + System.Convert.ToString(mainVerClient) + "_to_" + System.Convert.ToString(mainVerClient + 1) + ".zip"), "tmp.zip");
                    }
                    mainVerClient++;
                    File.WriteAllText(@".\" + clientVerFile, System.Convert.ToString(mainVerClient));
                }
                catch (WebException)
                {
                    textBox1.Text = "Download Error";
                }
            }
            else
            {
                textBox1.Text = "Up to date!";
                button1.Enabled = true;
            }
        }

        // Load DLL functions for controling window
        public const int WM_NCLBUTTONDOWN = 0xA1;
        public const int HT_CAPTION = 0x2;

        [System.Runtime.InteropServices.DllImportAttribute("user32.dll")]
        public static extern int SendMessage(IntPtr hWnd, int Msg, int wParam, int lParam);
        [System.Runtime.InteropServices.DllImportAttribute("user32.dll")]
        public static extern bool ReleaseCapture();

        // Allows user to move window
        private void pictureBox2_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                ReleaseCapture();
                SendMessage(Handle, WM_NCLBUTTONDOWN, HT_CAPTION, 0);
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            // TODO: Check if client.exe exists before executing
            System.Diagnostics.Process.Start("Client.exe", launchArgs);
            Environment.Exit(0);
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
        }


        void wc_DownloadCompleted(object sender, AsyncCompletedEventArgs e)
        {
            // Extract patch now that download is complete
            backgroundWorker1.RunWorkerAsync();
        }

        // Updates the progress bar when downloading
        void wc_DownloadProgressChanged(object sender, DownloadProgressChangedEventArgs e)
        {
            progressBar1.Value = e.ProgressPercentage;
        }

        // Majority of patching done here and RunWorkerCompleted
        private void backgroundWorker1_DoWork(object sender, DoWorkEventArgs e)
        {
            // Extract new patch
            using (ZipArchive archive = ZipFile.Open(@".\tmp.zip", ZipArchiveMode.Update))
            {
                archive.ExtractToDirectory(@".\", true);
            }
            File.Delete(@".\tmp.zip");
        }

        private void backgroundWorker1_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            // Continue downloading patches as needed
            if (mainVerClient < mainVer)
            {
                if (mainVerClient + 1 != mainVer)
                    textBox1.Text = "Updating " + Convert.ToString(mainVerClient + 1) + " to " + Convert.ToString(mainVerClient + 2);
                try
                {
                    using (var newPatch = new WebClient())
                    {
                        newPatch.DownloadFileCompleted += wc_DownloadCompleted;
                        newPatch.DownloadProgressChanged += wc_DownloadProgressChanged;
                        newPatch.DownloadFileAsync(new Uri(patchURL + System.Convert.ToString(mainVerClient) + "_to_" + System.Convert.ToString(mainVerClient + 1) + ".zip"), "tmp.zip");
                    }
                    mainVerClient++;
                    File.WriteAllText(@".\" + clientVerFile, System.Convert.ToString(mainVerClient));
                }
                catch (WebException)
                {
                    textBox1.Text = "Download Error";
                }
            }
            else
            {
                textBox1.Text = "Up to date!";
                button1.Enabled = true;
            }
        }
    }

    // Inherit ExtractToDirectory from ZipArchiveExtensions 
    public static class ZipArchiveExtensions
    {
        // This function overwrites files in the directory when setting the bool argument to true
        public static void ExtractToDirectory(this ZipArchive archive, string destinationDirectoryName, bool overwrite)
        {
            if (!overwrite)
            {
                archive.ExtractToDirectory(destinationDirectoryName);
                return;
            }
            foreach (ZipArchiveEntry file in archive.Entries)
            {
                string completeFileName = Path.Combine(destinationDirectoryName, file.FullName);
                string directory = Path.GetDirectoryName(completeFileName);

                if (!Directory.Exists(directory))
                    Directory.CreateDirectory(directory);

                if (file.Name != "")
                    file.ExtractToFile(completeFileName, true);
            }
        }
    }
}
