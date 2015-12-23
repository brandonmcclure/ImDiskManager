using System;
using System.Collections.Generic;
using System.Collections;
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
        public static Objects.ProgramSettings GlobalVars = new Objects.ProgramSettings();

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

        public static ArrayList GetAvailableDrives()
        {
            ArrayList DriveLetters = new ArrayList(26);
            for (int i = 65; i < 91; i++)
            {
                DriveLetters.Add(Convert.ToChar(i));
            }

            System.IO.DriveInfo[] drives = System.IO.DriveInfo.GetDrives();

            foreach (var drive in drives)
            {

                DriveLetters.Remove(drive.Name);
            }
            return DriveLetters;
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


            GlobalVars.DriveLetter = MainSettings.GlobalStrings["DriveLetter"];
            GlobalVars.ImagePath = MainSettings.GlobalStrings["ImagePath"];
            GlobalVars.ExitFlag = MainSettings.GlobalBooleans["ExitFlag"];
            GlobalVars.ReadyToMap = MainSettings.GlobalBooleans["ReadytoMap"];
            GlobalVars.Timeout = 10;

            Logger.WriteToLog("App started at " + DateTime.Now);

            //Start the UI window in a seperate thread
            Thread uiThread = new Thread(new ThreadStart(StartThread));
            uiThread.Start();

            //Application Loop
            do
            {
                if (GlobalVars.ReadyToMap == true)
                {
                    if (!CheckDriveStatus(GlobalVars.DriveLetter))
                    {
                        ProcessStartInfo startInfo = new ProcessStartInfo();
                        startInfo.CreateNoWindow = false;
                        startInfo.UseShellExecute = false;
                        startInfo.FileName = "Imdisk";
                        startInfo.WindowStyle = ProcessWindowStyle.Hidden;
                        startInfo.Arguments = "-a -t vm -f " + GlobalVars.ImagePath + " -m " + GlobalVars.DriveLetter + ": -b auto";
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
                        while (GlobalVars.Timeout > i)
                        {
                            if (CheckDriveStatus(GlobalVars.DriveLetter))
                            {
                                Logger.WriteToLog("Drive has been mapped.");
                                i = GlobalVars.Timeout + 1;
                            }
                            else
                            {
                                Logger.WriteToLog("Drive is still not mapped.");
                                System.Threading.Thread.Sleep(1000);
                                i++;
                            }

                            Logger.WriteToLog("We could not map the drive");
                            Thread.MemoryBarrier();
                            GlobalVars.ReadyToMap = false;
                        }
                    }
                    else
                    {
                        Logger.WriteToLog("Drive has already been mapped. Aborting.");
                    }
                }

                System.Threading.Thread.Sleep(1000);
            } while (GlobalVars.ExitFlag == false);

            Logger.WriteToLog("App ended at " + DateTime.Now);
            return 1;
        }
    }
}
