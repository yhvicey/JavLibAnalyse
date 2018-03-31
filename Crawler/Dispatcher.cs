using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Crawler
{
    public static class Dispatcher
    {
        public static bool IsProcessorTaskCompleted => ProcessorTasks.IsCompleted;
        public static bool IsProducerTaskCompleted => ProducerTasks.IsCompleted;

        public static void AddProcessorTask(string id)
        {
            if (ProcessorTaskHistory.Contains(id))
                return;
            ProcessorTasks.Add(id);
        }

        public static void AddProducerTask(string genres, int page)
        {
            if (ProducerTaskHistory.Contains((genres, page)))
                return;
            ProducerTasks.Add((genres, page));
        }

        public static void FinishProcessorTask(string id)
        {
            ProcessorTaskHistory.Add(id);
            if (++CheckpointCounter >= Config.CheckpointThreshold)
            {
                Checkpoint();
                CheckpointCounter = 0;
            }
        }

        public static string GetProcessorTask()
        {
            var task = ProcessorTasks.Take();
            ProcessorTaskHistory.Add(task);
            if (++CheckpointCounter > Config.CheckpointThreshold)
                Checkpoint();
            return task;
        }

        public static (string Genres, int Page) GetProducerTask()
        {
            var task = ProducerTasks.Take();
            ProducerTaskHistory.Add(task);
            return task;
        }

        public static void ReAddProcessorTask(string id)
            => ProcessorTasks.Add(id);

        public static void ReAddProducerTask(string genres, int page)
            => ProducerTasks.Add((genres, page));

        private static int CheckpointCounter = 0;
        private static readonly HashSet<string> ProcessorTaskHistory = new HashSet<string>();
        private static readonly HashSet<(string, int)> ProducerTaskHistory = new HashSet<(string, int)>();
        private static readonly BlockingCollection<string> ProcessorTasks = new BlockingCollection<string>();
        private static readonly BlockingCollection<(string, int)> ProducerTasks = new BlockingCollection<(string, int)>();

        static Dispatcher()
        {
            var checkpointFiles = Directory.GetFiles(Config.TempDir, "checkpoint_*.chkpnt").ToList();
            if (checkpointFiles.Count == 0) return;
            checkpointFiles.Sort();
            var checkpointFile = checkpointFiles.Last();
            try
            {
                foreach (var id in File.ReadAllLines(checkpointFile))
                    ProcessorTaskHistory.Add(id);
            }
            catch (Exception ex)
            {
                Logger.Error($"Failed to load checkpoint file. File path: {checkpointFile}.", ex);
            }
        }

        private static void Checkpoint()
        {
            CheckpointCounter = 0;
            var checkpointFilePath = $"{Config.TempDir}/checkpoint_{DateTime.Now:yyyy_MM_dd_HH_mm_ss_ffff}.chkpnt";
            try
            {
                File.WriteAllLines(checkpointFilePath, ProcessorTaskHistory);
            }
            catch (Exception ex)
            {
                Logger.Error($"Failed to create checkpoint file. File path: {checkpointFilePath}.", ex);
            }
        }
    }
}