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
                ProcessorTasks.Add(id);
            }
        }

        public static void AddProducerTask(string genres, int page)
        {
            lock (ProducerSyncLock)
            {
                if (IsProducerTaskFinished(genres, page))
                    return;
                ProducerTasks.Add((genres, page));
            }
        }

        public static void FinishProducerTask(string genres, int page)
        {
            if (IsProducerTaskFinished(genres, page)) return;
            lock (ProducerSyncLock)
            {
                ProducerTaskHistory[genres] = page;
            }
            if (++CheckpointCounter >= Config.CheckpointThreshold)
                Checkpoint();
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
        private static readonly Dictionary<string, int> ProducerTaskHistory = new Dictionary<string, int>();
        private static readonly BlockingCollection<(string, int)> ProducerTasks = new BlockingCollection<(string, int)>();

        private static int CheckpointCounter = 0;

        static Dispatcher()
        {
            var checkpointFiles = Directory.GetFiles(Config.TempDir, "checkpoint_*").ToList();
            if (checkpointFiles.Count == 0) return;
            checkpointFiles.Sort();
            var checkpointFile = checkpointFiles.Last();
            try
            {
                foreach (var record in File.ReadAllLines(checkpointFile))
                {
                    var sessions = record.Split(",");
                    ProducerTaskHistory.Add(sessions.First(), int.Parse(sessions.Last()));
                }
            }
            catch (Exception ex)
            {
                Logger.Error($"Failed to load checkpoint file. File path: {checkpointFile}.", ex);
            }
        }

        private static void Checkpoint()
        {
            CheckpointCounter = 0;
            var checkpointFilePath = $"{Config.TempDir}/checkpoint_{DateTime.Now:yyyy_MM_dd_HH_mm_ss_ffff}";
            try
            {
                File.WriteAllLines(checkpointFilePath, ProducerTaskHistory.Select(record => $"{record.Key},{record.Value}"));
            }
            catch (Exception ex)
            {
                Logger.Error($"Failed to create checkpoint file. File path: {checkpointFilePath}.", ex);
            }
        }

    }
}