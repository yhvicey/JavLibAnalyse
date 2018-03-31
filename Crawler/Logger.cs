using System;
using System.IO;

namespace Crawler
{
    public enum LogLevel
    {
        Debug,
        Info,
        Error,
        Fatal
    }

    public static class Logger
    {
        public static string LogFilePath { get; } = $"{Config.LogDir}/{DateTime.Now:yyyyMMdd}.log";
        public static LogLevel LogLevel { get; set; } =
#if DEBUG
            LogLevel.Debug;
#else
            LogLevel.Info;
#endif

        public static void Debug(Exception ex, string message = null)
        {
            if (LogLevel > LogLevel.Debug || ex == null) return;
            Log(FormatMessage(LogLevel.Debug, $"StackTrace: {ex.StackTrace}"));
            if (message == null) return;
            Log(FormatMessage(LogLevel.Debug, message));
        }

        public static void Info(string message)
        {
            if (LogLevel > LogLevel.Info) return;
            Log(FormatMessage(LogLevel.Info, message));
        }

        public static void Error(string message, Exception ex = null)
        {
            if (LogLevel > LogLevel.Error) return;
            Log(FormatMessage(LogLevel.Error, message));
            if (ex == null) return;
            Log(FormatMessage(LogLevel.Error, $"{ex.GetType().Name}: {ex.Message}"));
            Debug(ex);
        }

        public static void Fatal(string message, Exception ex = null)
        {
            if (LogLevel > LogLevel.Fatal) return;
            Log(FormatMessage(LogLevel.Fatal, message));
            if (ex == null) return;
            Log(FormatMessage(LogLevel.Fatal, $"{ex.GetType().Name}: {ex.Message}"));
            Debug(ex);
        }

        private static readonly object SyncLock = new object();

        static Logger()
        {
            if (!File.Exists(LogFilePath)) return;
            var i = 0;
            while (File.Exists($"{LogFilePath}.{i}"))
                i++;
            File.Move(LogFilePath, $"{LogFilePath}.{i}");
        }

        private static string FormatMessage(LogLevel logLevel, string message) => $"[{DateTime.Now:yyyy-MM-dd HH:mm:ss}][{logLevel}] {message}";

        private static void Log(params string[] message)
        {
            lock (SyncLock)
            {
                foreach (var msg in message)
                {
                    Console.WriteLine(msg);
                }
                if (!File.Exists(LogFilePath))
                {
                    File.Create(LogFilePath).Close();
                }
                File.AppendAllLines(LogFilePath, message);
            }
        }
    }
}