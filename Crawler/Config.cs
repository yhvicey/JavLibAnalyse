using System;
using System.IO;
using Newtonsoft.Json.Linq;

namespace Crawler
{
    public static class Config
    {
        public static int CheckpointThreshold { get; }
        public static bool DownloadImage { get; }
        public static string Genres { get; }
        public static int InfoTimerInterval { get; }
        public static int MaxRequestInterval { get; }
        public static int MaxThreadCount { get; }
        public static int MinRequestInterval { get; }
        public static string LogDir { get; }
        public static LogLevel LogLevel { get; }
        public static string OutputDir { get; }
        public static int ProcessorCount { get; }
        public static int ProducerCount { get; }
        public static string RootUrl { get; }
        public static string TempDir { get; }

        public static void PrintConfig()
        {
            Logger.Info($"========== Config Start ==========");
            Logger.Info($"CheckpointThreshold:  {CheckpointThreshold}");
            Logger.Info($"DownloadImage:        {DownloadImage}");
            Logger.Info($"Genres:               {Genres}");
            Logger.Info($"InfoTimerInterval:    {InfoTimerInterval}");
            Logger.Info($"MaxRequestInterval:   {MaxRequestInterval}");
            Logger.Info($"MaxThreadCount:       {MaxThreadCount}");
            Logger.Info($"MinRequestInterval:   {MinRequestInterval}");
            Logger.Info($"LogDir:               {LogDir}");
            Logger.Info($"LogLevel:             {LogLevel}");
            Logger.Info($"OutputDir:            {OutputDir}");
            Logger.Info($"ProcessorCount:       {ProcessorCount}");
            Logger.Info($"ProducerCount:        {ProducerCount}");
            Logger.Info($"RootUrl:              {RootUrl}");
            Logger.Info($"TempDir:              {TempDir}");
            Logger.Info($"==========  Config End  ==========");
        }

        private static readonly string ConfigFilePath = $"config.json";

        static Config()
        {
            try
            {
                var content = File.Exists(ConfigFilePath) ? File.ReadAllText(ConfigFilePath) : throw new Exception("No config file!");
                var config = JObject.Parse(content);

                LogDir = config["logDir"]?.Value<string>() ?? $"log";
                OutputDir = config["outputDir"]?.Value<string>() ?? $"data";
                TempDir = config["tempDir"]?.Value<string>() ?? $"temp";

                if (!Directory.Exists(LogDir)) Directory.CreateDirectory(LogDir);
                if (!Directory.Exists(OutputDir)) Directory.CreateDirectory(OutputDir);
                if (!Directory.Exists(TempDir)) Directory.CreateDirectory(TempDir);

                Genres = config["genres"]?.Value<string>() ??
                    throw new Exception("Missing genres in config file!");

                CheckpointThreshold = config["checkpointThreshold"]?.Value<int>() ?? 100;
                DownloadImage = config["downloadImage"]?.Value<bool>() ?? false;
                InfoTimerInterval = config["infoTimerInterval"]?.Value<int>() ?? 60;
                LogLevel = (LogLevel)Enum.Parse(typeof(LogLevel), config["logLevel"]?.Value<string>() ?? "info", true);
                ProcessorCount = config["processorCount"]?.Value<int>() ?? 4;
                MaxRequestInterval = config["maxRequestInterval"]?.Value<int>() ?? 300;
                MaxThreadCount = config["maxThreadCount"]?.Value<int>() ?? 65535;
                MinRequestInterval = config["minRequestInterval"]?.Value<int>() ?? 60;
                ProducerCount = config["producerCount"]?.Value<int>() ?? 4;
                RootUrl = config["rootUrl"]?.Value<string>() ?? "http://www.19lib.com/cn";
            }
            catch (Exception ex)
            {
                Logger.Fatal("Error occured while loading config.", ex);
                Environment.Exit(-1);
            }
        }
    }
}