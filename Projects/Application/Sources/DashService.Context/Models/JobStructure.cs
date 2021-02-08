using System.Threading;
using System.Threading.Tasks;
using DashService.Framework;
using DashService.Job.Abstraction;

namespace DashService.Context.Models
{
    public class JobStructure : IJobStructure
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
        public IPluggedinAssemblyModel PluggedinAssembly
        {
            get;
            set;
        }

        public JobStructure(IJob jobInstance, IPluggedinAssemblyModel pluggedinAssembly)
        {
            JobInstance = jobInstance;
            PluggedinAssembly = pluggedinAssembly;

            StartCancellationTokenSource = new CancellationTokenSource();
            StopCancellationTokenSource = new CancellationTokenSource();
            JobStatus = JobStatus.None;
        }
    }
}
