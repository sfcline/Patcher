using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Reflection;
using System.Net;
using System.IO;
using System.IO.Compression;

namespace Launcher
{
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            prepatch();

            // Loads DLL and opens Windows Forms Application from DLL
            Assembly assembly = Assembly.LoadFile(Directory.GetCurrentDirectory() + "\\PatchDLL.dll");
            Type type = assembly.GetType("WindowsFormsApp1.Form1");
            Form form = (Form)Activator.CreateInstance(type);
            Application.Run(form);
        }

        // Updates PatchDLL to latest version. 
        // TODO: Provide a GUI when downloading an update.
        // TODO: Provide an easier way to change update URL and search using well defined variables
        static void prepatch()
        {
            // Defining variables
            string html = string.Empty;
            String url = "http://cdn.mabiclassic.com/patch/patch.txt";
            StreamReader patchver = new StreamReader("patchver.ini");
            int patchDllVer = int.Parse(patchver.ReadLine());
            int mainVer = 0;
            patchver.Close();

            // Grab patch.txt and assign values
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);
            using (HttpWebResponse response = (HttpWebResponse)request.GetResponse())
            using (Stream stream = response.GetResponseStream())
            using (StreamReader reader = new StreamReader(stream))
            {
                while (reader.Peek() >= 0)
                {
                    html = reader.ReadLine();
                    if (html[0] == 'p' && html[1] == 'a' && html[2] == 't' && html[3] == 'c' && html[4] == 'h' && html[5] == 'e')
                        mainVer = int.Parse(html.Remove(0, 16));
                }
            }

            // Update if needed
            if (patchDllVer < mainVer)
            {
                try
                {
                    // Download new dll
                    using (var newPatch = new WebClient())
                    {
                        newPatch.DownloadFile(new Uri("http://cdn.mabiclassic.com/patch/patcher.zip"), "tmp.zip");
                    }
                    // Extract new dll
                    using (ZipArchive archive = ZipFile.Open(@".\tmp.zip", ZipArchiveMode.Update))
                    {
                        archive.ExtractToDirectory(@".\"); // TODO: Force extraction to overwrite
                    }
                    File.Delete(@".\tmp.zip");
                }
                catch (WebException)
                {
                    // TODO: add an error message in GUI
                }
            }
        }
    }
}
