using System.Threading;

namespace DashService.Worker.Framework
{
    public class JobDetails
    {
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
