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
        public Form1()
        {
            InitializeComponent();
        }

        // All patching setup done here
        private void Form1_Load(object sender, EventArgs e)
        {
            // Defining variables

            // Grab patch.txt and assign values

            // Call function to start downloading files if not up to date
            
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
            System.Diagnostics.Process.Start("Client.exe", " code:1622 ver:126 logip:142.44.223.136 logport:11000 chatip:142.44.223.136 chatport:8002 setting:\"file://data/features.xml=Regular, USA\"");
            Environment.Exit(0);
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
        }
        
        // Majority of patching will be done here and RunWorkerCompleted
        private void backgroundWorker1_DoWork(object sender, DoWorkEventArgs e)
        {
            // Download new patch
        }

        private void backgroundWorker1_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            // Extract new patch
        }
    }
}
