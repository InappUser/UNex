using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using System.ComponentModel;
using System.IO.Compression;
using System.IO;
using System.Threading;
namespace updater
{
    class Program
    {

        static string dirName = "UNex-updates";
        static string dataName = "UNex_Data";
        static string gameName = "UNex.exe";
        static void Main(string[] args)
        {
            Console.WriteLine("Preparing to download update");
            Thread.Sleep(1000);
            WebClient Client = new WebClient();
            Client.DownloadFile(new Uri("https://github.com/InappUser/UNex/archive/updates.zip"), "updates.zip");
            unzip();
        }

        static void complete(object sender, AsyncCompletedEventArgs e)
        {

            if (!File.Exists("updates.zip"))
            {
                complete(sender, e);
            }

            unzip();
        }

        static void cleanUp()
        {
            if (Directory.Exists(dirName))
            {

                Console.WriteLine("Removing old shit");
                Thread.Sleep(1000);
                Directory.Delete(dirName, true);
            }

            if (Directory.Exists(dataName))
            {
                Console.WriteLine("Removing old data files");
                Thread.Sleep(1000);
                Directory.Delete(dataName, true);
            }

        }


        static void unzip()
        {

            cleanUp();

            string cd = Directory.GetCurrentDirectory();
            string extractPath = @cd;
            string fileName = "updates.zip";

            try
            {
                Console.WriteLine("Extracting da ZIP. Cuz we're da real MVPs");
                Thread.Sleep(1000);
                ZipFile.ExtractToDirectory(fileName, extractPath);
                rename();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                Thread.Sleep(60000);
            }

        }

        static void rename()
        {
            if (!Directory.Exists(dirName))
            {
                Console.WriteLine("The directory " + dirName + " does not exist. exiting.");
                Thread.Sleep(3000);
                Environment.Exit(0);
            }

            Console.WriteLine("Refreshing directory");
            Thread.Sleep(1000);
            Directory.Move(dirName, dataName);
            finish();
        }

        static void launchGame()
        {
            Console.WriteLine("Launching game....");
            Thread.Sleep(2000);
            System.Diagnostics.Process.Start(gameName);
            Thread.Sleep(1000);
            Environment.Exit(0);
        }

        static void finish()
        {

            Console.WriteLine("Update applied successfully. Enjoy the game. :)");
            Thread.Sleep(4000);
            Console.Clear();
            launchGame();
        }

    }
}
