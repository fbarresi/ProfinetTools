using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProfinetMonitor.Logging
{
    using System;
    using System.Diagnostics;
    using System.IO;
    using System.Linq;

    internal class RollingFileTraceListener : TraceListener
    {

        public string FileName { get; set; }
        public int MaxFilesize { get; set; } = 512000;  // 512 KB
        public int MaxFileCount { get; set; } = 5;
        private TextWriterTraceListener TraceListener;
        private object Lock = new object();
        private int Count = 1000; // Start at 1000 to check size and file-count on first written message,after that every 1000 MSG

        public RollingFileTraceListener(string fileName)
        {
            FileName = fileName;
            var File = new FileInfo(fileName);

            // Create Stream manually to open "Shareable" so to enable reading while writing
            // If an IO Exception is raised, probably the application is already running. In this case create an new 
            // filename with an perpending GUID
            // On Error Fall back to default system
            try
            {
                try
                {
                    var Stream = File.Open(FileMode.Append, FileAccess.Write, FileShare.Read);
                    TraceListener = new TextWriterTraceListener(Stream);
                }

                catch (IOException ex) // The log file was probably already open for writing
                {
                    File = new FileInfo(File.FullName + ";" + Guid.NewGuid().ToString() + ".log");

                    var Stream = File.Open(FileMode.Append, FileAccess.Write, FileShare.Read);
                    TraceListener = new TextWriterTraceListener(Stream);
                }
            }

            catch (Exception ex) // Something unexpected happened like Access rights or similar
            {
                TraceListener = new TextWriterTraceListener(fileName);
            }

            if (!File.Directory.Exists)
                File.Directory.Create();
        }

        public override void Write(string message)
        {
            lock (Lock)
            {
                TraceListener.Write(message);
                TraceListener.Flush();

                CheckSizeAndCount();
            }
        }

        public override void WriteLine(string message)
        {
            lock (Lock)
            {
                TraceListener.WriteLine(message);
                TraceListener.Flush();

                CheckSizeAndCount();
            }
        }

        private void CheckSizeAndCount()
        {
            if (Count % 1000 == 0)
            {
                lock (Lock)
                {
                    CheckTraceFileSize();
                    CheckTraceFileCount();
                    Count = 0;
                }
            }
            else
            {
                Count += 1;
            }
        }

        private void CheckTraceFileSize()
        {
            var TraceFile = new FileInfo(FileName);
            if (!TraceFile.Exists)
                return;
            if (TraceFile.Length >= MaxFilesize)
            {
                TraceListener.Flush();
                TraceListener.Close();

                var MoveToFile = new FileInfo(TraceFile.FullName + ";" + DateTime.Now.ToString("dd-MM-yyyy-hh-mm-ss") + ".log");
                if (MoveToFile.Exists) // if it already exists, then this means we filled up an log file in one second! so something is clearly wrong
                {
                    MoveToFile = new FileInfo(TraceFile.FullName + ";" + DateTime.Now.ToString("dd-MM-yyyy-hh-mm-ss") + ";" + Guid.NewGuid().ToString() + ".log");
                }

                TraceFile.MoveTo(MoveToFile.FullName);
                TraceListener = new TextWriterTraceListener(FileName);
            }
        }

        private void CheckTraceFileCount()
        {
            try
            {
                var TraceFile = new FileInfo(FileName);
                if (!TraceFile.Directory.Exists)
                    return;
                var Files = TraceFile.Directory.GetFiles(TraceFile.Name + "*", SearchOption.TopDirectoryOnly);

                if (Files.Length >= MaxFileCount)
                {
                    var Ordered = Files.OrderByDescending(a => a.LastWriteTime);
                    int Cnt = Ordered.Count();
                    while (Cnt > MaxFileCount)
                    {
                        Ordered.ElementAtOrDefault(Cnt - 1).Delete();
                        Cnt -= 1;
                    }
                }
            }
            catch (Exception ex)
            {
                return;
            } // we ignore errors
        }
    }
}
