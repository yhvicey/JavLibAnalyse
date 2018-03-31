using System;
using System.IO;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Crawler
{
    public static class Config
    {
        public static int CheckpointThreshold { get; }
        public static string Genres { get; }
        public static int MaxRequestInterval { get; }
        public static int MinRequestInterval { get; }
        public static string LogDir { get; }
        public static string OutputDir { get; }
        public static int ProcessorCount { get; }
        public static int ProducerCount { get; }
        public static string RootUrl { get; }
        public static string TempDir { get; }

        private static readonly string ConfigFilePath = $"config.json";

        static Config()
        {
            try
            {
                var content = File.Exists(ConfigFilePath) ? File.ReadAllText(ConfigFilePath) : "{}";
                var config = JObject.Parse(content);

                LogDir = config["logDir"]?.Value<string>() ?? $"log";
                OutputDir = config["outputDir"]?.Value<string>() ?? $"data";
                TempDir = config["outputDir"]?.Value<string>() ?? $"temp";

                if (!Directory.Exists(LogDir)) Directory.CreateDirectory(LogDir);
                if (!Directory.Exists(OutputDir)) Directory.CreateDirectory(OutputDir);
                if (!Directory.Exists(TempDir)) Directory.CreateDirectory(TempDir);

                Genres = config["genres"]?.Value<string>() ??
                    throw new Exception("Missing genres in config file!");

                CheckpointThreshold = config["checkpointThreshold"]?.Value<int>() ?? 100;
                ProcessorCount = config["processorCount"]?.Value<int>() ?? 4;
                MaxRequestInterval = config["maxRequestInterval"]?.Value<int>() ?? 300;
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