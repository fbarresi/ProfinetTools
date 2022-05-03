using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProfinetMonitor.Logging
{
    public enum LoggingLevel
    {
        Debug = 0,
        Info = 2,
        Warning = 4,
        Error = 8,
        Critical = 16,
        None = 128
    }

    /// <summary>
    /// Helper Class that logs information to the default Trace Listeners
    /// </summary>
    /// <remarks></remarks>
    public class Logger
    {
        /// <summary>
        /// the current loggers category or name as identification in the log file
        /// </summary>
        /// <value></value>
        /// <returns></returns>
        /// <remarks></remarks>
        private string Category { get; set; } = "";

        /// <summary>
        /// Create new logger for an category. An category can be any user defined string
        /// </summary>
        /// <param name="category"></param>
        public Logger(string category)
        {
            Category = category;
        }

        private void Write(string message, string category, LoggingLevel LoggingLevel)
        {
            if (!IsAboveThreshold(LoggingLevel)) return;

            var SB = new StringBuilder();
            SB.Append("\t");
            SB.Append(DateTime.Now.ToString());
            SB.Append("\t");
            SB.Append(LoggingLevel.ToString());
            SB.Append("\t");
            SB.Append(Process.GetCurrentProcess().Id);
            SB.Append("\t");
            SB.Append(System.Threading.Thread.CurrentThread.ManagedThreadId + ": " + System.Threading.Thread.CurrentThread.Name);
            SB.Append("\t");
            SB.Append(message);

            Trace.Write(SB.ToString() + Environment.NewLine, category);
        }

        private void Write(object obj, string category, LoggingLevel LoggingLevel)
        {
            if (!IsAboveThreshold(LoggingLevel))
                return;

            var Message = new StringBuilder();
            if (obj is Exception) // Special handling for exceptions so they spill more info into the log files
            {
                Exception Ex = (Exception)obj;
                Message.Append(Ex.Message);
                Message.Append("\t");
                Message.Append(Ex.ToString());
            }
            else
            {
                Message.Append(obj.ToString());
            }

            Write(Message.ToString() + Environment.NewLine, category, LoggingLevel);
        }

        #region Logging Methods
        /// <summary>
        /// Writes an Informational Message to the log listeners
        /// </summary>
        /// <param name="message">The message to be logged</param>
        /// <remarks></remarks>
        public void Info(string message)
        {
            Write(message, Category, LoggingLevel.Info);
        }

        /// <summary>
        /// Writes an informational message to the log listeners
        /// </summary>
        /// <param name="obj">the objects .toString method that will be logged</param>
        /// <remarks></remarks>
        public void Info(object obj)
        {
            Write(obj, Category, LoggingLevel.Info);
        }

        /// <summary>
        /// Writes an informational message to the log listeners
        /// </summary>
        /// <param name="message">The message to be logged</param>
        /// <param name="args">the objects .toString method that will be logged</param>
        /// <remarks></remarks>
        public void Info(string message, params object[] args)
        {
            Write(string.Format(message, args), Category, LoggingLevel.Info);
        }

        /// <summary>
        /// Writes an debug Message to the log listeners
        /// </summary>
        /// <param name="message">The message to be logged</param>
        /// <remarks></remarks>
        public void Debug(string message)
        {
            Write(message, Category, LoggingLevel.Debug);
        }

        /// <summary>
        /// Writes an debug message to the log listeners
        /// </summary>
        /// <param name="obj">the objects .toString method that will be logged</param>
        /// <remarks></remarks>
        public void Debug(object obj)
        {
            Write(obj, Category, LoggingLevel.Debug);
        }

          /// <summary>
        /// Writes an debug message to the log listeners
        /// </summary>
        /// <param name="message">The message to be logged</param>
        /// <param name="args">the objects .toString method that will be logged</param>
        /// <remarks></remarks>
        public void Debug(string message, params object[] args)
        {
            Write(string.Format(message, args), Category, LoggingLevel.Debug);
        }

        /// <summary>
        /// Writes an warning message to the log listeners
        /// </summary>
        /// <param name="message">The message to be logged</param>
        /// <remarks></remarks>
        public void Warning(string message)
        {
            Write(message, Category, LoggingLevel.Warning);
        }

        /// <summary>
        /// Writes an warning message to the log listeners
        /// </summary>
        /// <param name="obj">the objects .toString method that will be logged</param>
        /// <remarks></remarks>
        public void Warning(object obj)
        {
            Write(obj, Category, LoggingLevel.Warning);
        }

        /// <summary>
        /// Writes an warning message to the log listeners
        /// </summary>
        /// <param name="message">The message to be logged</param>
        /// <param name="args">the objects .toString method that will be logged</param>
        /// <remarks></remarks>
        public void Warning(string message, params object[] args)
        {
            Write(string.Format(message, args), Category, LoggingLevel.Warning);
        }

        /// <summary>
        /// Writes an error message to the log listeners
        /// </summary>
        /// <param name="message">The message to be logged</param>
        /// <remarks></remarks>
        public void Error(string message)
        {
            Write(message, Category, LoggingLevel.Error);
        }

        /// <summary>
        /// Writes an error message to the log listeners
        /// </summary>
        /// <param name="obj">the objects .toString method that will be logged</param>
        /// <remarks></remarks>
        public void Error(object obj)
        {
            Write(obj, Category, LoggingLevel.Error);
        }

        /// <summary>
        /// Writes an error message to the log listeners
        /// </summary>
        /// <param name="message">The message to be logged</param>
        /// <param name="args">the objects .toString method that will be logged</param>
        /// <remarks></remarks>
        public void Error(string message, params object[] args)
        {
            Write(string.Format(message, args), Category, LoggingLevel.Error);
        }

        /// <summary>
        /// Writes an critical error message to the log listeners
        /// </summary>
        /// <param name="message">The message to be logged</param>
        /// <remarks></remarks>
        public void Critical(string message)
        {
            Write(message, Category, LoggingLevel.Critical);
        }

        /// <summary>
        /// Writes an critical error message to the log listeners
        /// </summary>
        /// <param name="obj">the objects .toString method that will be logged</param>
        /// <remarks></remarks>
        public void Critical(object obj)
        {
            Write(obj, Category, LoggingLevel.Critical);
        }

        /// <summary>
        /// Writes an critical error message to the log listeners
        /// </summary>
        /// <param name="message">The message to be logged</param>
        /// <param name="args">the objects .toString method that will be logged</param>
        /// <remarks></remarks>
        public void Critical(string message, params object[] args)
        {
            Write(string.Format(message, args), Category, LoggingLevel.Critical);
        }

        #endregion

        /// <summary>
        /// Checks if the message must be logged, depending on the logging level
        /// </summary>
        /// <param name="loggingLevel"></param>
        /// <returns></returns>
        /// <remarks></remarks>
        private bool IsAboveThreshold(LoggingLevel loggingLevel)
        {
            return loggingLevel >= LogManager.LoggingThreshold;
        }
    }
}
