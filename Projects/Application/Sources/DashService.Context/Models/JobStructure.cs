using DashService.Context.Framework;
using DashService.Job.Abstraction;
using System.Threading;

namespace DashService.Context.Models
{
    public class JobStructure
    {
        public IJob JobInstance
        {
            get;
            set;
        }

        public JobStatus JobStatus
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
    }
}
