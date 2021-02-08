using System.Threading;
using System.Threading.Tasks;
using DashService.Job.Abstraction;

namespace DashService.Framework
{
    public interface IJobStructure
    {
        IJob JobInstance
        {
            get;
            set;
        }

        CancellationTokenSource StartCancellationTokenSource
        {
            get;
            set;
        }

        CancellationTokenSource StopCancellationTokenSource
        {
            get;
            set;
        }

        Task JobStartingTask
        {
            get;
            set;
        }

        Task JobStoppingTask
        {
            get;
            set;
        }

        JobStatus JobStatus
        {
            get;
            set;
        }

        IPluggedinAssemblyModel PluggedinAssembly
        {
            get;
            set;
        }
    }
}
