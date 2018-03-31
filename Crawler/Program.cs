using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading;
using System.Threading.Tasks;

namespace Crawler
{
    public static class Program
    {
        static void Main(string[] args)
        {
            SetMaxThreadCount(Config.MaxThreadCount);

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
            while (true)
            {
                var (genres, page) = Dispatcher.GetProducerTask();
                Producer.Produce(genres, page);
                Thread.Sleep(random.Next(Config.MinRequestInterval * 1000, Config.MaxRequestInterval * 1000));
            }
        }

        private static void ProcessorMainProc()
        {
            var random = new Random(Guid.NewGuid().GetHashCode());
            while (true)
            {
                var task = Dispatcher.GetProcessorTask();
                var result = Processor.Process(task);
                if (result == null)
                    continue;
                if (Saver.Save(task, result))
                    Dispatcher.FinishProcessorTask(task);
                Thread.Sleep(random.Next(Config.MinRequestInterval * 1000, Config.MaxRequestInterval * 1000));
            }
        }

        private static void SetMaxThreadCount(int maxThreadCount)
        {
            ThreadPool.SetMaxThreads(maxThreadCount, maxThreadCount);
        }
    }
}