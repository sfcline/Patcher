using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace PatchDLL
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        // The majority of patching setup will be done below
        private void Form1_Load(object sender, EventArgs e)
        {
            // Define variables



            // Grab patch info and assign values



            // Start download initial patch if needed



        }

        // Majority of patching will be done between DoWork and RunWorkerCompleted
        private void backgroundWorker1_DoWork(object sender, DoWorkEventArgs e)
        {

        }

        private void backgroundWorker1_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {

        }
    }
}
