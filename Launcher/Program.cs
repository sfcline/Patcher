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
        // URL holding the current, up to date, patcher
        const string updateURL = "http://cdn.mabiclassic.com/patch/patcher.zip";

        // URL holding the current patcher version
        const string patchInfoURL = "http://cdn.mabiclassic.com/patch/patch.txt";

        // Argument to retrieve remote patcher version from patchInfoURL
        const string patchInfoType = "patcher_version";

        // File holding the local patcher version
        const string patchVerFile = "patchver.ini";

        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            prepatch();

            // Loads DLL and opens Windows Forms Application from DLL
            Assembly assembly = Assembly.LoadFile(Directory.GetCurrentDirectory() + "\\PatchDLL.dll");
            Type type = assembly.GetType("PatchDLL.Form1");
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
            StreamReader patchver = new StreamReader(patchVerFile);
            int patchDllVer = int.Parse(patchver.ReadLine());
            int mainVer = 0;
            patchver.Close();

            // Grab patch.txt and assign values
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(patchInfoURL);
            using (HttpWebResponse response = (HttpWebResponse)request.GetResponse())
            using (Stream stream = response.GetResponseStream())
            using (StreamReader reader = new StreamReader(stream))
            {
                while (reader.Peek() >= 0)
                {
                    html = reader.ReadLine();
                    if (html[0] == patchInfoType[0] && html[1] == patchInfoType[1] && html[2] == patchInfoType[2] && html[3] == patchInfoType[3] && html[4] == patchInfoType[4] && html[5] == patchInfoType[5])
                        mainVer = int.Parse(html.Remove(0, 16));
                }
            }

            // Update if needed
            if (patchDllVer < mainVer)
            {
                // Download new dll
                using (var newPatch = new WebClient())
                {
                    newPatch.DownloadFile(new Uri(updateURL), "tmp.zip");
                }
                // Extract new dll
                using (ZipArchive archive = ZipFile.Open(@".\tmp.zip", ZipArchiveMode.Update))
                {
                    archive.ExtractToDirectory(@".\", true);
                }
                File.Delete(@".\tmp.zip");
                File.WriteAllText(@".\" + patchVerFile, System.Convert.ToString(mainVer));
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
