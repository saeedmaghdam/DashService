using System.Threading;
using System.Threading.Tasks;
using DashService.Framework;

namespace DashService.JobHandler.Models
{
    public class JobInstance : IJobInstance
    {
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
        public IJobAssembly JobAssembly
        {
            get;
            set;
        }

        public JobInstance(IJobAssembly jobAssembly)
        {
            JobAssembly = jobAssembly;

            StartCancellationTokenSource = new CancellationTokenSource();
            StopCancellationTokenSource = new CancellationTokenSource();
            JobStatus = JobStatus.None;
        }
    }
}
