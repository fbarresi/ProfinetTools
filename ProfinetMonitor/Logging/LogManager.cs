using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace ProfinetMonitor.Logging
{
    internal static class LogManager
    {
        public static string LogFileName { get; } = System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "ProfinetMonitor.log");
        private static bool InitialSetupDone= false;
        public static LoggingLevel LoggingThreshold { get; set; } = LoggingLevel.Debug;

        public static void SetUpLogging()
        {
            if (InitialSetupDone) return;
            InitialSetupDone = true;

            var TL = new RollingFileTraceListener(LogFileName);
            Trace.AutoFlush = true;
            Trace.Listeners.Add(TL);

            WriteProcessStartLog();
            AppDomain.CurrentDomain.UnhandledException += UnhandledExceptionOccured;
            AppDomain.CurrentDomain.ProcessExit += WriteProcessExitLog;
        }

        private static Logger DefLogger = new Logger("ProfinetMonitor");

        private static void UnhandledExceptionOccured(object sender, UnhandledExceptionEventArgs e)
        {
            DefLogger.Critical(e.ExceptionObject);
        }

        private static void WriteProcessExitLog(object sender, EventArgs e)
        {
            DefLogger.Info("Exit Application with exit code {0}", Environment.ExitCode);
        }

        private static void WriteProcessStartLog()
        {
            DefLogger.Info(string.Format("Starting Application '{0}', Version '{1}'", Assembly.GetExecutingAssembly().GetName().Name, Assembly.GetExecutingAssembly().GetName().Version));
            DefLogger.Info("Command line was: {0}", Environment.CommandLine);
        }

    }
}
