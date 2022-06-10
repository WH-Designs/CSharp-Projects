using System;
using System.Collections.Concurrent;
using System.Threading;

namespace FoodCart
{
    public class FoodieCart
    {
        private static int _numberOfWorkers;
        private static double _timeBetweenCustomers;
        private static int _averageServiceTime;

        public static Semaphore Workers;
        public static BlockingCollection<Thread> Line = new();
        private static Random _random = new Random();
        public static DateTime ServingTime;
        public static Mutex ServingTimeMutex = new();
        public static DateTime CurrentTime;
        public static int NumberOfCustomers;
        public static double CustomerStartServingTime;
        public static double CustomerEndServingTime;
        public static double TotalServingTime;

        public FoodieCart(int numberOfWorkers, double timeBetweenCustomers, int averageServiceTime)
        {
            _numberOfWorkers = numberOfWorkers;
            _timeBetweenCustomers = timeBetweenCustomers;
            _averageServiceTime = averageServiceTime;

            Workers = new Semaphore(numberOfWorkers, numberOfWorkers);

            CurrentTime = DateTime.Now;
        }

        private static void AddCustomer(object obj)
        {
            var current = DateTime.Now;
            var end = current.AddSeconds(70);
            var i = 0;
            while (current < end)
            {
                var customer = new Thread(CustomerProcess)
                {
                    Name = $"{++i}"
                };

                Line.Add(customer);
                Console.WriteLine($"At time {Convert.ToInt32((DateTime.Now - CurrentTime).TotalSeconds)}, customer {customer.Name} arrives in line");

                Thread.Sleep(_random.Next(2000, ((int) _timeBetweenCustomers * 1000)));

                current = DateTime.Now;
            }
        }

        public static void CustomerProcess(object obj)
        {
            try
            {
                Workers.WaitOne();

                while (DateTime.Now < ServingTime) ;

                Console.WriteLine($"At time {Convert.ToInt32((DateTime.Now - CurrentTime).TotalSeconds)}, customer {Thread.CurrentThread.Name} starts being served");
                CustomerStartServingTime = (DateTime.Now - CurrentTime).TotalSeconds;

                Thread.Sleep(4000);

                ServingTimeMutex.WaitOne();
                ServingTime = DateTime.Now.AddSeconds(8);
                ServingTimeMutex.ReleaseMutex();

                //When the customer leaves
                Console.WriteLine($"At time {Convert.ToInt32((DateTime.Now - CurrentTime).TotalSeconds)}, customer {Thread.CurrentThread.Name} leaves the food cart");
                CustomerEndServingTime = (DateTime.Now - CurrentTime).TotalSeconds;

                TotalServingTime += CustomerEndServingTime - CustomerStartServingTime;

                NumberOfCustomers++;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                throw;
            }
            finally
            {
                Workers.Release();
            }
        }

        public void Run()
        {
            var current = DateTime.Now;
            var end = current.AddSeconds(70);

            Console.WriteLine($"Mean inter-arrival time: {_timeBetweenCustomers}");
            Console.WriteLine($"Mean service time: {_averageServiceTime}");
            Console.WriteLine($"Number of workers: {_numberOfWorkers}");
            Console.WriteLine($"Length of simulation: 70\n");

            var lineThread = new Thread(AddCustomer);
            lineThread.Start();

            ServingTime = DateTime.Now.AddSeconds(10);

            while (current < end)
            {
                Line.TryTake(out var customer);
                customer?.Start();

                current = DateTime.Now;
            }

            foreach (var customer in Line)
            {
                if (customer.IsAlive) customer.Join();
            }

            Console.WriteLine($"\nSimulation terminated after {NumberOfCustomers} customers served");
            Console.WriteLine($"Average waiting time = {Math.Round(TotalServingTime/NumberOfCustomers, 1)}");
        }
    }
}