using DashService.Job.Abstraction;
using System.Threading;
using System.Threading.Tasks;

namespace DashService.Context.Models
{
    public class JobStructure
    {
        public IJob JobInstance
        {
            get;
            set;
        }

        public CancellationTokenSource StartCancellationTokenSource
        {
            get;
            set;
        }

        public CancellationTokenSource StopCancellationTokenSource
        {
            get;
            set;
        }

        public Task JobStartingTask
        {
            get;
            set;
        }

        public Task JobStoppingTask
        {
            get;
            set;
        }

        public JobStatus JobStatus
        {
            get;
            set;
        }
    }

    public enum JobStatus
    {
        None = 0,
        Stopped = 1,
        Running = 2,
        Paused = 3
    }

}
