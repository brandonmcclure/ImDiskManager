using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;
using HelperFuncs;
using System.Diagnostics;   //Used to run the IMDisk command line command
using System.Windows.Forms; //Windows Form

namespace ImDiskManager
{
    class Program
    {
        public static SettingHandler MainSettings = new SettingHandler();

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

        [STAThread]
        private static void StartThread()
        {
            Form myForm = new MainForm();
            Application.EnableVisualStyles();
            Application.Run(myForm); 
        }

        [STAThread]
        static int Main(string[] args)
        {
            
            Dictionary<string, string> argumentMapping = new Dictionary<string, string>();

            argumentMapping.Add("-Drive=", "DriveLetter");
            argumentMapping.Add("-ImagePath=", "ImagePath");

            MainSettings.GlobalBooleans.Add("NeedsUpdated", false);
            MainSettings.GlobalBooleans.Add("ReadytoMap", false);
            MainSettings.GlobalStrings.Add("DriveLetter", "Z");
            MainSettings.GlobalStrings.Add("ImagePath", "C:\\Users\\bmcclure\\Documents\\Ramdisk.img");
            MainSettings.GlobalBooleans.Add("ExitFlag", false);

            //Actually parse the arguments
            MainSettings = ArgumentParser.parseArgs(args, MainSettings, argumentMapping);

            string DriveLetter = MainSettings.GlobalStrings["DriveLetter"];
            string ImagePath = MainSettings.GlobalStrings["ImagePath"];
            Boolean exitFlag = MainSettings.GlobalBooleans["ExitFlag"];
            Boolean ReadytoMap = MainSettings.GlobalBooleans["ReadytoMap"];
            Int32 Timeout = 10;

            Logger.WriteToLog("App started at " + DateTime.Now);

            //Start the UI window in a seperate thread
            Thread uiThread = new Thread(new ThreadStart(StartThread));
            uiThread.Start();

            //Application Loop
            do
            {
                Boolean needsUpdated = false;
                MainSettings.GlobalBooleans.TryGetValue("NeedsUpdated", out needsUpdated);
                if ( needsUpdated == true)
                {
                    DriveLetter = MainSettings.GlobalStrings["DriveLetter"];
                    ImagePath = MainSettings.GlobalStrings["ImagePath"];
                    exitFlag = MainSettings.GlobalBooleans["ExitFlag"];
                    ReadytoMap = MainSettings.GlobalBooleans["ReadytoMap"];
                }

                if (ReadytoMap == true)
                {
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
                }

                System.Threading.Thread.Sleep(1000);
            } while (exitFlag == false);

            Logger.WriteToLog("App ended at " + DateTime.Now);
            return 1;
        }
    }
}
