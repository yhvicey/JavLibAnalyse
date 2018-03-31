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
            SetMaxThreadCount(Config.ProcessorCount + Config.ProducerCount);

            var producerTasks = LaunchThreads(Config.ProducerCount, ProducerMainProc);
            var processorTasks = LaunchThreads(Config.ProcessorCount, ProcessorMainProc);

            var tasks = producerTasks.Union(processorTasks).ToArray();
            Task.WaitAll(tasks);
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
            foreach (var genres in Config.Genres.Split(";"))
            {
                Dispatcher.AddProducerTask(genres, 1);
            }
            var random = new Random(Guid.NewGuid().GetHashCode());
            while (!Dispatcher.IsProcessorTaskCompleted)
            {
                //Thread.Sleep(random.Next(Config.MinRequestInterval * 1000, Config.MaxRequestInterval * 1000));
                var (genres, page) = Dispatcher.GetProducerTask();
                Producer.Produce(genres, page);
            }
        }

        private static void ProcessorMainProc()
        {
            var random = new Random(Guid.NewGuid().GetHashCode());
            while (!Dispatcher.IsProcessorTaskCompleted)
            {
                //Thread.Sleep(random.Next(Config.MinRequestInterval * 1000, Config.MaxRequestInterval * 1000));
                var task = Dispatcher.GetProcessorTask();
                var result = Processor.Process(task);
                if (result == null)
                    continue;
                if (Saver.Save(task, result))
                    Dispatcher.FinishProcessorTask(task);
            }
        }

        private static void SetMaxThreadCount(int maxThreadCount)
        {
            ThreadPool.GetMinThreads(out var minWorkerThreadCount, out var minIOThreadCount);
            maxThreadCount = Math.Max(Math.Max(minWorkerThreadCount, minIOThreadCount), maxThreadCount);
            ThreadPool.SetMaxThreads(maxThreadCount, maxThreadCount);
        }
    }
}