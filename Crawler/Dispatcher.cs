using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;

namespace Crawler
{
    public static class Dispatcher
    {
        public static bool IsComplete => ProducerTasks.Count == 0 && ProcessorTasks.Count == 0;

        public static void AddProcessorTask(string id)
        {
            lock (ProcessorSyncLock)
            {
                if (IsProcessorTaskFinished(id))
                    return;
                ProcessorTasks.Add(id);
            }
        }

        public static void AddProducerTask(string genres, int page)
        {
            lock (ProducerSyncLock)
            {
                if (ProducerTaskHistory.Contains((genres, page)))
                    return;
                ProducerTasks.Add((genres, page));
            }
        }

        public static void FinishProducerTask(string genres, int page)
        {
            lock (ProducerSyncLock)
            {
                ProducerTaskHistory.Add((genres, page));
            }
        }

        public static string GetProcessorTask()
            => ProcessorTasks.Take();

        public static (string Genres, int Page) GetProducerTask()
            => ProducerTasks.Take();

        public static bool IsProcessorTaskFinished(string task)
            => Directory.Exists($"{Config.OutputDir}/{task}");

        public static void PrintInfo()
        {
            Logger.Info($"========== Current Info ==========");
            Logger.Info($"Processor task count:             {ProcessorTasks.Count}");
            Logger.Info($"Producer task count:              {ProducerTasks.Count}");
            Logger.Info($"Finished processor task count:    {Directory.GetDirectories(Config.OutputDir).Length}");
            Logger.Info($"Finished producer task count:     {ProducerTaskHistory.Count}");
            Logger.Info($"==================================");
        }

        public static void ReAddProcessorTask(string id)
            => ProcessorTasks.Add(id);

        public static void ReAddProducerTask(string genres, int page)
            => ProducerTasks.Add((genres, page));

        private static readonly object ProcessorSyncLock = new object();
        private static readonly BlockingCollection<string> ProcessorTasks = new BlockingCollection<string>();
        private static readonly object ProducerSyncLock = new object();
        private static readonly HashSet<(string, int)> ProducerTaskHistory = new HashSet<(string, int)>();
        private static readonly BlockingCollection<(string, int)> ProducerTasks = new BlockingCollection<(string, int)>();
    }
}