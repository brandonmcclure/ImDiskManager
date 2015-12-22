using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HelperFuncs;
using System.Diagnostics;   //Used to run the IMDisk command line command

namespace ImDiskManager
{
    class Program
    {
        static Boolean CheckDriveStatus(string driveName)
        {
            Boolean isReady = false;
            System.IO.DriveInfo[] drives = System.IO.DriveInfo.GetDrives();

            foreach (var drive in drives)
            {
                if (drive.Name.Contains(driveName))
                {
                    isReady = true;
                }
            }

            return isReady;
        }

        static int Main(string[] args)
        {
            SettingHandler MainSettings = new SettingHandler();
            Dictionary<string, string> argumentMapping = new Dictionary<string, string>();

            argumentMapping.Add("-Drive=", "DriveLetter");
            //argumentMapping.Add("DriveLetter", "/Drive=");
            argumentMapping.Add("-ImagePath=", "ImagePath");

            MainSettings.GlobalStrings.Add("DriveLetter", "Z");
            MainSettings.GlobalStrings.Add("ImagePath", "C:\\Users\\bmcclure\\Documents\\Ramdisk.img");

            //Actually parse the arguments
            MainSettings = ArgumentParser.parseArgs(args, MainSettings, argumentMapping);

            string DriveLetter = MainSettings.GlobalStrings["DriveLetter"];
            string ImagePath = MainSettings.GlobalStrings["ImagePath"];
            Int32 Timeout = 10; 

            Logger.WriteToLog("App started at " + DateTime.Now);

            

            
            /*
            foreach(string arg in args)
            {
                if (arg.Contains("-Drive="))
                {
                    //Validate the enviroment argument
                    DriveLetter = arg.Remove(0, 7);
                }
                else if (arg.Contains("-ImagePath="))
                {
                    ImagePath = arg.Remove(0, 11);
                }

            }
             * */

            if (!CheckDriveStatus(DriveLetter))
            {
                ProcessStartInfo startInfo = new ProcessStartInfo();
                startInfo.CreateNoWindow = false;
                startInfo.UseShellExecute = false;
                startInfo.FileName = "Imdisk";
                startInfo.WindowStyle = ProcessWindowStyle.Hidden;
                startInfo.Arguments = "-a -t vm -f " + ImagePath + " -m " + DriveLetter + ": -b auto";
                try
                {
                    using (Process exeProcess = Process.Start(startInfo))
                    {
                        exeProcess.WaitForExit();
                    }
                }
                catch (Exception ex)
                {
                    Logger.WriteToLog(ex.Message);
                }

                int i = 0;
                while (Timeout > i)
                {
                    if (CheckDriveStatus(DriveLetter))
                    {
                        Logger.WriteToLog("Drive has been mapped.");
                        i = Timeout + 1;
                    }
                    else
                    {
                        Logger.WriteToLog("Drive is still not mapped.");
                        System.Threading.Thread.Sleep(1000);
                        i++;
                    }
                }
            }
            else
            {
                Logger.WriteToLog("Drive has already been mapped. Aborting.");
            }

            Logger.WriteToLog("App ended at " + DateTime.Now);
            return 1;
        }
    }
}
