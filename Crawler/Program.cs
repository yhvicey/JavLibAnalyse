﻿using System;
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
            var timer = LaunchInfoTimer();

            foreach (var genres in Config.Genres.Split(";"))
            {
                Dispatcher.AddProducerTask(genres, 1);
            }
            Logger.Info($"Launching {Config.ProducerCount} producer threads...");
            var producerTasks = LaunchThreads(Config.ProducerCount, ProducerMainProc);
            Logger.Info($"Launching {Config.ProcessorCount} processor threads...");
            var processorTasks = LaunchThreads(Config.ProcessorCount, ProcessorMainProc);

            var tasks = producerTasks.Union(processorTasks).ToArray();
            Task.WaitAll(tasks);
            Console.ReadLine();
        }

        private static CancellationTokenSource _cancelTokenSource = new CancellationTokenSource();

        private static Timer LaunchInfoTimer()
        {
            return new Timer(state =>
            {
                Dispatcher.PrintInfo();
                if (!Dispatcher.IsComplete)
                    return;

                _cancelTokenSource.Cancel();
                Logger.Info($"All tasks finished. Application stopped.");
                Environment.Exit(0);
            }, null, Config.InfoTimerInterval * 1000, Config.InfoTimerInterval * 1000);
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
            while (!_cancelTokenSource.IsCancellationRequested)
            {
                var (genres, page) = Dispatcher.GetProducerTask();
                if (Producer.Produce(genres, page))
                    Logger.Info($"Producer task ({genres}, {page}) finished successful.");
                Thread.Sleep(random.Next(Config.MinRequestInterval * 1000, Config.MaxRequestInterval * 1000));
            }
        }

        private static void ProcessorMainProc()
        {
            var random = new Random(Guid.NewGuid().GetHashCode());
            while (!_cancelTokenSource.IsCancellationRequested)
            {
                var task = Dispatcher.GetProcessorTask();
                if (Processor.Process(task))
                    Logger.Info($"Processor task {task} finished successful.");
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