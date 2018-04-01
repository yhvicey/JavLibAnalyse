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
        public static string ErrorLogFilePath { get; } = $"{Config.LogDir}/{DateTime.Now:yyyyMMdd}.err";
        public static string InfoLogFilePath { get; } = $"{Config.LogDir}/{DateTime.Now:yyyyMMdd}.log";

        public static void Info(string message)
        {
            if (Config.LogLevel > LogLevel.Info) return;
            LogInfo(FormatMessage(LogLevel.Info, message));
        }

        public static void Error(string message, Exception ex = null)
        {
            if (Config.LogLevel > LogLevel.Error) return;
            LogError(FormatMessage(LogLevel.Error, message));
            if (ex == null) return;
            LogError(FormatMessage(LogLevel.Error, $"{ex.GetType().Name}: {ex.Message}"));
            Debug(ex);
        }

        public static void Fatal(string message, Exception ex = null)
        {
            if (Config.LogLevel > LogLevel.Fatal) return;
            LogError(FormatMessage(LogLevel.Fatal, message));
            if (ex == null) return;
            LogError(FormatMessage(LogLevel.Fatal, $"{ex.GetType().Name}: {ex.Message}"));
            Debug(ex);
        }

        private static readonly object InfoSyncLock = new object();
        private static readonly object ErrorSyncLock = new object();

        static Logger()
        {
            if (File.Exists(InfoLogFilePath))
            {
                var i = 0;
                while (File.Exists($"{InfoLogFilePath}.{i}"))
                    i++;
                File.Move(InfoLogFilePath, $"{InfoLogFilePath}.{i}");
            }
            if (File.Exists(ErrorLogFilePath))
            {
                var i = 0;
                while (File.Exists($"{ErrorLogFilePath}.{i}"))
                    i++;
                File.Move(ErrorLogFilePath, $"{ErrorLogFilePath}.{i}");
            }
        }

        private static void Debug(Exception ex, string message = null)
        {
            if (Config.LogLevel > LogLevel.Debug || ex == null) return;
            LogError(FormatMessage(LogLevel.Debug, $"StackTrace: {ex.StackTrace}"));
            if (message == null) return;
            LogError(FormatMessage(LogLevel.Debug, message));
        }

        private static string FormatMessage(LogLevel logLevel, string message) => $"[{DateTime.Now:yyyy-MM-dd HH:mm:ss}][{logLevel}] {message}";

        private static void LogError(params string[] message)
        {
            lock (ErrorSyncLock)
            {
                foreach (var msg in message)
                {
                    Console.Error.WriteLine(msg);
                }
                if (!File.Exists(ErrorLogFilePath))
                {
                    File.Create(ErrorLogFilePath).Close();
                }
                File.AppendAllLines(ErrorLogFilePath, message);
            }
        }

        private static void LogInfo(params string[] message)
        {
            lock (InfoSyncLock)
            {
                foreach (var msg in message)
                {
                    Console.WriteLine(msg);
                }
                if (!File.Exists(InfoLogFilePath))
                {
                    File.Create(InfoLogFilePath).Close();
                }
                File.AppendAllLines(InfoLogFilePath, message);
            }
        }
    }
}