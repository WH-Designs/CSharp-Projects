using System;
using System.Threading;

namespace AmusementPark
{
    public class MutexDemo
    {
        private static readonly Mutex _mutexLock = new Mutex();
        public void Run()
        {
            var mutexDemo = new MutexDemo();

            mutexDemo.GenerateThreads();
        }

        private void GenerateThreads()
        {
            for (var i = 0; i < 10; i++)
            {
                var thread = new Thread(ThreadProcess)
                {
                    Name = $"Thread {i + 1}"
                };
                thread.Start();
            }
        }

        private void ThreadProcess(object obj)
        {
            _mutexLock.WaitOne();

            Console.WriteLine($"{Thread.CurrentThread.Name} has entered the line.");

            Thread.Sleep(1000);

            Console.WriteLine($"{Thread.CurrentThread.Name} is done and leaving.");

            _mutexLock.ReleaseMutex();
        }
    }
}