using Coordinator.Lib.Interfaces;

namespace Coordinator.Lib
{
    public class PreemptiveShortestJobScheduler : IScheduler
    {
        public IList<IJob> IoQueue { get; set; }

        public IJob PeekNext()
        {
            return IoQueue.First();
        }

        public void Reschedule(IJob job)
        {
            IoQueue.Prepend(job);
        }

        public void Schedule(IJob job)
        {
            foreach(var io_job in IoQueue)
            {
                if (job.BurstTime < io_job.BurstTime)
                {
                    IoQueue.Prepend(job);
                }
                else if (job.BurstTime == io_job.BurstTime)
                {
                    if (job.ArrivalTime < io_job.ArrivalTime)
                    {
                        IoQueue.Append(job);
                    }
                    else
                    {
                        IoQueue.Prepend(job);
                    }
                }
                else
                {
                    IoQueue.Append(job);
                }
            }
        }

        public IJob TakeNext()
        {
            var next_job = IoQueue.Take(1);

            return next_job;
        }
    }
}
