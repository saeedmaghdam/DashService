using System.Threading;
using System.Threading.Tasks;

namespace DashService.Framework
{
    public interface IJobInstance
    {
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

        IJobAssembly JobAssembly
        {
            get;
            set;
        }
    }
}
