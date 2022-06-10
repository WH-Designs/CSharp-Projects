using Coordinator.Lib.Interfaces;

namespace Coordinator.Lib
{
    public class JobQueue : IJobQueue
    {
        public IList<IJob> Queue { get; set; }

        public IJob Dequeue()
        {
            return Queue.RemoveAt(0);
        }

        public void Enqueue(IJob job)
        {
            Queue.Prepend(job);
        }

        public bool IsEmpty()
        {
            if (Queue.Count() <= 0)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        public IJob Peek()
        {
            return Queue.First();
        }
        public bool AnyJobHaveAShorterBurstTime(int burstTime)
        {
            bool result = false;
            foreach (var job in Queue)
            {
                if (job.BurstTime <= burstTime)
                {
                    result = true;
                }
                else if (job.BurstTime > burstTime)
                {
                    result = false;
                }
            }
            return result;
        }
    }
}
