using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;

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
                if (ProcessorTasks.Contains(id))
                    return;
                ProcessorTasks.Add(id);
            }
        }

        public static void AddProducerTask(string genres, int page)
        {
            lock (ProducerSyncLock)
            {
                if (IsProducerTaskFinished(genres, page))
                    return;
                if (ProducerTasks.Contains((genres, page)))
                    return;
                ProducerTasks.Add((genres, page));
            }
        }

        public static bool Checkpoint()
        {
            var checkpointTime = DateTime.Now;
            var processorTasksCheckpointFilePath = $"{Config.TempDir}/checkpoint_processor_current_{checkpointTime:yyyy_MM_dd_HH_mm_ss_ffff}";
            var producerTasksCheckpointFilePath = $"{Config.TempDir}/checkpoint_producer_current_{checkpointTime:yyyy_MM_dd_HH_mm_ss_ffff}";
            var producerTaskHistoryCheckpointFilePath = $"{Config.TempDir}/checkpoint_producer_history_{checkpointTime:yyyy_MM_dd_HH_mm_ss_ffff}";
            try
            {
                File.WriteAllLines(processorTasksCheckpointFilePath, ProcessorTasks);
                File.WriteAllLines(producerTasksCheckpointFilePath, ProducerTasks.Select(record => $"{record.Item1},{record.Item2}"));
                File.WriteAllLines(producerTaskHistoryCheckpointFilePath, ProducerTaskHistory.Select(record => $"{record.Key},{record.Value}"));
                return true;
            }
            catch (Exception ex)
            {
                Logger.Error($"Failed to create checkpoint file. Checkpoint time: {checkpointTime:yyyy-MM-dd HH:mm:ss.ffff}", ex);
                return false;
            }
        }


        public static void FinishProducerTask(string genres, int page)
        {
            if (IsProducerTaskFinished(genres, page)) return;
            lock (ProducerSyncLock)
            {
                ProducerTaskHistory[genres] = page;
            }
        }

        public static string GetProcessorTask()
            => ProcessorTasks.Take();

        public static (string Genres, int Page) GetProducerTask()
            => ProducerTasks.Take();

        public static bool IsProcessorTaskFinished(string task)
            => Directory.Exists($"{Config.OutputDir}/{task}");

        public static bool IsProducerTaskFinished(string genres, int page)
        {
            return ProducerTaskHistory.ContainsKey(genres)
                ? ProducerTaskHistory[genres] >= page
                : false;
        }

        public static void PrintInfo()
        {
            Logger.Info($"========== Current Info ==========");
            Logger.Info($"Processor task count:             {ProcessorTasks.Count}");
            Logger.Info($"Producer task count:              {ProducerTasks.Count}");
            Logger.Info($"Finished processor task count:    {Directory.GetDirectories(Config.OutputDir).Length}");
            Logger.Info($"Finished producer task count:     {ProducerTaskHistory.Values.Sum()}");
            Logger.Info($"==================================");
        }

        public static void ReAddProcessorTask(string id)
            => ProcessorTasks.Add(id);

        public static void ReAddProducerTask(string genres, int page)
            => ProducerTasks.Add((genres, page));

        private static readonly object ProcessorSyncLock = new object();
        private static readonly BlockingCollection<string> ProcessorTasks = new BlockingCollection<string>();
        private static readonly object ProducerSyncLock = new object();
        private static readonly Dictionary<string, int> ProducerTaskHistory = new Dictionary<string, int>();
        private static readonly BlockingCollection<(string, int)> ProducerTasks = new BlockingCollection<(string, int)>();

        static Dispatcher()
        {
            var processorTasksCheckpointFiles = Directory.GetFiles(Config.TempDir, "checkpoint_processor_current_*").ToList();
            var producerTasksCheckpointFiles = Directory.GetFiles(Config.TempDir, "checkpoint_producer_current_*").ToList();
            var producerTaskHistoryCheckpointFiles = Directory.GetFiles(Config.TempDir, "checkpoint_producer_history_*").ToList();
            processorTasksCheckpointFiles.Sort();
            producerTasksCheckpointFiles.Sort();
            producerTaskHistoryCheckpointFiles.Sort();

            if (processorTasksCheckpointFiles.Count != 0)
            {
                var processorTasksCheckpointFile = processorTasksCheckpointFiles.Last();
                try
                {
                    foreach (var record in File.ReadAllLines(processorTasksCheckpointFile))
                    {
                        ProcessorTasks.Add(record);
                    }
                }
                catch (Exception ex)
                {
                    Logger.Error($"Failed to load processor task checkpoint file. File path: {processorTasksCheckpointFile}.", ex);
                }
            }

            if (producerTasksCheckpointFiles.Count != 0)
            {
                var producerTasksCheckpointFile = producerTasksCheckpointFiles.Last();
                try
                {
                    foreach (var record in File.ReadAllLines(producerTasksCheckpointFile))
                    {
                        var sessions = record.Split(",");
                        var genres = sessions.First();
                        var page = int.Parse(sessions.Last());
                        ProducerTasks.Add((genres, page));
                    }
                }
                catch (Exception ex)
                {
                    Logger.Error($"Failed to load processor task checkpoint file. File path: {producerTasksCheckpointFile}.", ex);
                }
            }

            if (producerTaskHistoryCheckpointFiles.Count != 0)
            {
                var producerTaskHistoryCheckpointFile = producerTaskHistoryCheckpointFiles.Last();
                try
                {
                    foreach (var record in File.ReadAllLines(producerTaskHistoryCheckpointFile))
                    {
                        var sessions = record.Split(",");
                        var genres = sessions.First();
                        var page = int.Parse(sessions.Last());
                        ProducerTaskHistory.Add(genres, page);
                        ProducerTasks.Add((genres, page + 1));
                    }
                }
                catch (Exception ex)
                {
                    Logger.Error($"Failed to load processor task checkpoint file. File path: {producerTaskHistoryCheckpointFile}.", ex);
                }
            }
        }
    }
}