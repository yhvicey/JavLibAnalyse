using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Crawler
{
    public static class Program
    {
        static void Main(string[] args)
        {
            Logger.Info("Application started.");
            SetMaxThreadCount(Config.MaxThreadCount);

            Config.PrintConfig();
            var checkpointTimer = LaunchCheckpointTimer();
            var infoTimer = LaunchInfoTimer();

            Logger.Info($"Launching {Config.ProducerCount} producer threads...");
            var producerTasks = LaunchThreads(Config.ProducerCount, ProducerMainProc);
            Logger.Info($"Launching {Config.ProcessorCount} processor threads...");
            var processorTasks = LaunchThreads(Config.ProcessorCount, ProcessorMainProc);

            var tasks = producerTasks.Union(processorTasks).ToArray();
            Task.WaitAll(tasks);
            Console.ReadLine();
        }

        private static CancellationTokenSource CancelTokenSource = new CancellationTokenSource();
        private static DateTime LastCheckTime = DateTime.Now;

        private static Timer LaunchCheckpointTimer()
        {
            return new Timer(state =>
            {
                if (Dispatcher.Checkpoint())
                    Logger.Info($"Checkpoint finished successfully.");
            }, null, 0, Config.CheckpointInterval * 1000);
        }

        private static Timer LaunchInfoTimer()
        {
            return new Timer(state =>
            {
                Dispatcher.PrintInfo();
                if (!Dispatcher.IsComplete)
                    LastCheckTime = DateTime.Now;

                if ((DateTime.Now - LastCheckTime).TotalSeconds <= Config.IdleTime) return;

                CancelTokenSource.Cancel();
                Logger.Info($"All tasks finished and waited for {Config.IdleTime} seconds. Application stopped.");
                Environment.Exit(0);
            }, null, 0, Config.InfoTimerInterval * 1000);
        }

        private static IEnumerable<Task> LaunchThreads(int threadCount, Action mainProc)
        {
            for (var i = 0; i < threadCount; i++)
            {
                yield return Task.Run(mainProc);
            }
        }

        private static void ProducerMainProc()
        {
            var random = new Random(Guid.NewGuid().GetHashCode());
            while (!CancelTokenSource.IsCancellationRequested)
            {
                var (genres, page) = Dispatcher.GetProducerTask();
                if (Dispatcher.IsProducerTaskFinished(genres, page))
                    continue;

                if (Producer.Produce(genres, page))
                    Logger.Info($"Producer task ({genres}, {page}) finished successfully.");
                Thread.Sleep(random.Next(Config.MinRequestInterval * 1000, Config.MaxRequestInterval * 1000));
            }
        }

        private static void ProcessorMainProc()
        {
            var random = new Random(Guid.NewGuid().GetHashCode());
            while (!CancelTokenSource.IsCancellationRequested)
            {
                var task = Dispatcher.GetProcessorTask();
                if (Dispatcher.IsProcessorTaskFinished(task))
                    continue;

                if (Processor.Process(task))
                    Logger.Info($"Processor task {task} finished successfully.");
                Thread.Sleep(random.Next(Config.MinRequestInterval * 1000, Config.MaxRequestInterval * 1000));
            }
        }

        private static void SetMaxThreadCount(int maxThreadCount)
        {
            ThreadPool.SetMaxThreads(maxThreadCount, maxThreadCount);
            Logger.Info($"Max thread count set to {maxThreadCount}");
        }
    }
}