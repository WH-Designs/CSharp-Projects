using Coordinator.Lib.Interfaces;

namespace Coordinator.Lib
{
    public class PriorityJob : IJob
    {
        public int Pid { get; set; }
        public int ArrivalTime { get; set; }
        public int BurstTime { get; set; }
        public int Priority { get; set; }
        public JobState JobState { get; set; }

        public int CompareTo(IJob? job)
        {
            return BurstTime.CompareTo(job?.BurstTime);
        }
    }
}
