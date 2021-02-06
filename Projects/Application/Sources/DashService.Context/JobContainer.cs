using DashService.Context.Models;
using DashService.Job.Abstraction;
using System.Collections.Generic;
using System.Threading;

namespace DashService.Context
{
    public static class JobContainer
    {
        private static List<JobStructure> _jobs = new List<JobStructure>();
        
        public static void Add(IEnumerable<IJob> jobs)
        {
            foreach (var job in jobs)
            {
                _jobs.Add(new JobStructure()
                {
                    JobInstance = job,
                    StartCancellationTokenSource = new CancellationTokenSource(),
                    StopCancellationTokenSource = new CancellationTokenSource(),
                    JobStatus = JobStatus.None
                });
            }
        }

        public static IEnumerable<JobStructure> Jobs => _jobs;
    }
}
