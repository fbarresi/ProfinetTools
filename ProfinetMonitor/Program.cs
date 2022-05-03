using ProfinetTools.Interfaces.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Windows.Forms;

namespace ProfinetMonitor
{
    static class Program
    {
        static Mutex mutex = new Mutex(true, "{8F6F0AC4-B9A1-45fd-A8CF-72F04E6BDE8F}");
        private static Logging.Logger Log = new Logging.Logger("ProfinetDeviceMonitor");

        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            Logging.LogManager.SetUpLogging();
            parseCommandline();

            if (monitorMode)
            {
                if (singleInstance)
                {
                    if (!mutex.WaitOne(TimeSpan.Zero, true))
                    {
                        Log.Info("Monitor was already running");
                        return;
                    }
                }

                Application.EnableVisualStyles();
                Application.SetCompatibleTextRenderingDefault(false);
                Application.Run(new MonitorModeDialog(MonitorInterval));
            }
            else
            {
                Application.EnableVisualStyles();
                Application.SetCompatibleTextRenderingDefault(false);
                Application.Run(new DevicesConfigurationDialog());
            }
            mutex.ReleaseMutex();
        }

        static bool singleInstance = false;
        static bool monitorMode = false;
        static int MonitorInterval;
        static private void parseCommandline()
        {
            singleInstance = Environment.CommandLine.ToLower().Contains("-once");
            monitorMode = Environment.CommandLine.ToLower().Contains("-monitor");

            if (monitorMode)
            {
                var tmp = Environment.CommandLine.ToLower();
                var idx = tmp.IndexOf("-monitor");
                if (idx < 0) //Monitor argument was not present
                {
                    monitorMode = false;
                    MonitorInterval = 0;
                    return;
                }

                //check if monitor has an value
                idx+= "-monitor".Length;
                idx = tmp.IndexOf("=", idx);
                if (idx < 0) //monitor did not have an value, so assume value 0
                {
                    MonitorInterval = 0;
                    return;
                }

                //parse the value off the commandline
                idx += "=".Length;
                var eidx = tmp.IndexOf(" ", idx);
                if (eidx < 0) tmp = tmp.Substring(idx);
                else tmp = tmp.Substring(idx, eidx-idx);

                MonitorInterval = int.Parse(tmp.Trim());
            }
        }

    }
}
