using System;
using System.Collections.Concurrent;
using System.Threading;

namespace AmusementPark
{
    public class FerrisWheel
    {
        private static object _loadendtimelock = new();

        private static readonly Mutex RideInMotion = new();
        private static readonly Semaphore Ride = new(5, 5);
        private static readonly BlockingCollection<Thread> Line = new();

        private static Mutex LoadEndTimeMutex = new();
        private static DateTime LoadTimeEnd;

        private const int MaxLoadTime = 5;
        private const int MaxRideTime = 5000;

        private static void QueueLine(object obj)
        {
            var current = DateTime.Now;
            var end = current.AddSeconds(20);
            var i = 0;
            while (current < end)
            {
                var guest = new Thread(GuestProcess)
                {
                    Name = $"Thread {++i}"
                };

                Line.Add(guest);

                Thread.Sleep(2000);

                current = DateTime.Now;
            }

            Console.WriteLine("The line is stopped");
        }

        private static void GuestProcess(object obj)
        {
            Ride.WaitOne();

            while (DateTime.Now < LoadTimeEnd) ;

            Console.WriteLine($"{Thread.CurrentThread.Name} has been seated.");

            Thread.Sleep(MaxRideTime);

            LoadEndTimeMutex.WaitOne();
            LoadTimeEnd = DateTime.Now.AddSeconds(MaxLoadTime);
            LoadEndTimeMutex.ReleaseMutex();

            //OR

/*          lock(_loadendtimelock)
            {
                LoadTimeEnd = DateTime.Now.AddSeconds(MaxLoadTime);
            }*/
            Ride.Release();
            Console.WriteLine("Ride stopped...");
        }

        public static void Main(string[] args)
        {
            var lineThread = new Thread(QueueLine);
            lineThread.Start();

            var current = DateTime.Now;
            var end = current.AddSeconds(25);

            LoadTimeEnd = DateTime.Now.AddSeconds(MaxLoadTime);

            while (current < end)
            {
                Line.TryTake(out var guest);
                guest?.Start();
                
                current = DateTime.Now;
            }
            foreach (var guest in Line)
            {
                if (guest.IsAlive) guest.Join();
            }
        }
    }
}

/* 
 RideInMotion.WaitOne();

                Line.TryTake(out var guest);

                guest?.Start();

                Ride.WaitOne();
                Console.WriteLine("Ride started....");

                Ride.Release();
                Console.WriteLine("Ride stopped....");



                RideInMotion.ReleaseMutex();
                current = DateTime.Now;*/


/*private const int MaxGuests = 20;
private const int MaxRideTime = 10;
private const int RideTimeToStart = 5;
public static Random Random { get; set; } = new Random();
public static double AverageWaitTime { get; set; } = 0;
public static Mutex AverageWaitTimeMutex { get; set; } = new Mutex();
public static Semaphore Ride { get; set; }
public static BlockingCollection<Guest> Guests { get; set; } = new BlockingCollection<Guest>();
public static int GuestNumber { get; set; } = 0;
public static Mutex GuestNumberMutex { get; set; } = new Mutex();

public static void GuestProcess(object param)
{
    var arrivalTime = Convert.ToDateTime(param);
    Console.WriteLine($"Guest {Thread.GetCurrentProcessorId()} arrived at {arrivalTime}");

    var waitTime = (DateTime.Now - arrivalTime).Seconds;

    Console.WriteLine($"Guest {Thread.GetCurrentProcessorId()} waited {waitTime}");

    AverageWaitTimeMutex.WaitOne();
    AverageWaitTime += waitTime;
    AverageWaitTimeMutex.ReleaseMutex();
}

public static void QueueLine(object simulaitonTime)
{
    var simulationEndTime = DateTime.Now.AddSeconds(Convert.ToInt32(simulaitonTime));

    while (DateTime.Now < simulationEndTime)
    {
        int guestNumber;

        GuestNumberMutex.WaitOne();
        guestNumber = ++GuestNumber;
        GuestNumberMutex.ReleaseMutex();

        var guest = new Guest
        {
            ArrivalTime = DateTime.Now,
            GuestNumber = guestNumber,
            Thread = new Thread(new ParameterizedThreadStart(GuestProcess))
        };

        guest.Thread.Start();
        Guests.Add(guest);

        Console.WriteLine($"Guest number {Thread.GetCurrentProcessorId()} entered the line.");
    }
}
public static void LoadRide()
{
    var numberOfGuests = 0;

    var timer = DateTime.Now;

    Console.WriteLine("Loading the ride with 20 guests.");

    while (numberOfGuests < Guests.Count && timer < timer.AddSeconds((double)RideTimeToStart))
    {
        Ride.WaitOne();

        ++numberOfGuests;

        Ride.Release();
    }

    Console.WriteLine($"The ride has been loaded/Ride Starting with {numberOfGuests} guests");

    Thread.Sleep(MaxRideTime * 1000);

    Console.WriteLine("Ride is stopping.");

}
static void Main(string[] args)
{
    *//*Console.WriteLine("Enter the simulation max run time: ");
    var maxRunTime = Convert.ToInt32(Console.ReadLine());

    Console.WriteLine("Enter the average time of arrival: ");
    var avergaeArrivalTime = Convert.ToDouble(Console.ReadLine());

    Ride = new Semaphore(MaxGuests, MaxGuests);

    var queueingLineThread = new Thread(new ParameterizedThreadStart(QueueLine));

    queueingLineThread.Start(maxRunTime);

    var simulationEndTime = DateTime.Now.AddSeconds(Convert.ToInt32(maxRunTime));

    while (DateTime.Now < simulationEndTime)
    {
        LoadRide();
    }*/

/*var mutexDemo = new MutexDemo();

mutexDemo.Run();*//*

var semaphoreDemo = new SemaphoreDemo();

semaphoreDemo.Run();*/

/*//Console.WriteLine("Ride is ready.");
            Ride.WaitOne();

            Console.WriteLine($"{Thread.CurrentThread.Name} has been seated.");

            RideInMotion.WaitOne();

            //Console.WriteLine($"Ride has started at {DateTime.Now}");

            Thread.Sleep(5000);

            RideInMotion.ReleaseMutex();
            Console.WriteLine("The ride is over!");

            Ride.Release();*/