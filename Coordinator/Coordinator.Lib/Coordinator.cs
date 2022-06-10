using Coordinator.Lib.Interfaces;
using System.Diagnostics;

namespace Coordinator.Lib
{
    public class Coordinator
    {
        private static readonly List<IJob> _inputJobs = new List<IJob>();
        private static JobQueue _queueReadyToRun = new JobQueue(); //Ready to run
        private static JobQueue _queueRunning = new JobQueue(); //Running
        private static JobQueue _queueWaitingIo = new JobQueue(); //Waiting on I/O
        private const int _ioCompleteChance = 4;
        private const int _ioRequestChance = 10;
        private static Random _random = new Random();
        private static IScheduler _scheduler;

        //stats to track
        private static int _jobProccessTime;
        private static List<int> _numberOfJobsRan;
        private static int _clock;
        private static int _totalElapsedTimePerJob;
        private static int _state1WaitTime; //Waiting - ready to run
        private static int _state2WaitTime; //Waiting for I/O
        private static int _state3WaitTime; //Running on the CPU
        private static int _longestJobTime;
        private static int _shortestJobTime;

        private static bool IsIoComplete()
        {
            if(_random.Next(1, 100) % _ioCompleteChance == 0) return true;
            else return false;
        }
        private static bool HasIoRequest()
        {
            if (_random.Next(1, 100) % _ioRequestChance == 0) return true;
            else return false;
        }
        private static void ReadJobs(string filename)
        {
            string[] lines = System.IO.File.ReadAllLines(filename);
            IJob job;

            foreach (string line in lines)
            {
                line.RemoveWhitespace();
                string[] newJob = line.Split(":");

                job.Pid = int.Parse(newJob[0]);
                job.ArrivalTime = int.Parse(newJob[1]);
                job.BurstTime = int.Parse(newJob[2]);
                job.Priority = int.Parse(newJob[3]);

                _inputJobs.Add(job);
            }
        }
        
        public static void Run(string[] args)
        {
            _clock = 0;
            _random.Next(1, 100);

            foreach (var job in _inputJobs)
            {
                job.JobState = JobState.New;
                _scheduler.Schedule(job);
                _queueReadyToRun.Enqueue(job);
            }

            while (!_queueReadyToRun.IsEmpty())
            {
                var current_job = _scheduler.TakeNext();

                while (true)
                {
                    _queueRunning.Enqueue(current_job);
                    while (!_queueWaitingIo.IsEmpty())
                    {
                        var status = IsIoComplete();
                        if (status == true)
                        {
                            _scheduler.Reschedule(current_job);
                            _queueReadyToRun.Enqueue(current_job);
                            _jobProccessTime++;
                        }
                    }

                    if (current_job.BurstTime - current_job.ArrivalTime == 0)
                    {
                        _queueReadyToRun.Dequeue();
                        current_job.JobState = JobState.Terminated;

                        _numberOfJobsRan.Add(_jobProccessTime);
                        break;
                    }
                    else if (_queueReadyToRun.AnyJobHaveAShorterBurstTime(current_job.BurstTime))
                    {
                        _queueRunning.Dequeue();
                        current_job.JobState = JobState.Preempted;
                        _scheduler.Reschedule(current_job);
                        _queueReadyToRun.Enqueue(current_job);
                        _jobProccessTime++;
                    }
                    else if (current_job.JobState == JobState.Running)
                    {
                        var status = HasIoRequest();
                        if (status == true)
                        {
                            _queueRunning.Dequeue();
                            _jobProccessTime++;
                            _queueWaitingIo.Enqueue(current_job);
                        }
                        else if (_clock == current_job.BurstTime)
                        {
                            current_job.JobState = JobState.Ready;
                            _jobProccessTime++;
                            _queueWaitingIo.Dequeue();
                            _scheduler.Reschedule(current_job);
                            _queueReadyToRun.Enqueue(current_job);
                        }
                    }

                    //Statistics

                    Console.WriteLine($"{_clock}:{current_job.Pid}:{current_job.BurstTime-_clock}:{IsIoComplete()}:{current_job.JobState}");

                    _totalElapsedTimePerJob++;
                    _longestJobTime = _numberOfJobsRan.Max();
                    _shortestJobTime = _numberOfJobsRan.Min();
                    _clock++;

                    if (current_job.JobState == JobState.New)
                    {
                        _scheduler.Reschedule(current_job);
                        _queueReadyToRun.Enqueue(current_job);
                        break;
                    }
                }
            }
            
        }
    }
}