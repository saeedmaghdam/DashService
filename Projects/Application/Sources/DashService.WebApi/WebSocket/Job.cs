using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace DashService.WebApi.WebSocket
{
    public static class Job
    {
        public static Task Start(Guid jobViewId)
        {
            var job = Context.JobContainer.Jobs.Where(x => x.JobInstance.GetType().GUID == jobViewId).SingleOrDefault();

            if (job == null)
                return Task.CompletedTask;

            if (job.JobStartingTask.Status != TaskStatus.Running)
            {
                job.StartCancellationTokenSource = new System.Threading.CancellationTokenSource();
                job.StopCancellationTokenSource = new System.Threading.CancellationTokenSource();
                job.JobStartingTask = Task.Run(() => { job.JobInstance.StartAsync(job.StartCancellationTokenSource.Token); }, CancellationToken.None);
            }

            return Task.CompletedTask;
        }

        public static Task Stop(Guid jobViewId)
        {   
            var job = Context.JobContainer.Jobs.Where(x => x.JobInstance.GetType().GUID == jobViewId).SingleOrDefault();

            if (job == null)
                return Task.CompletedTask;

            if (job.JobStartingTask.Status == TaskStatus.Running)
            {
                job.StartCancellationTokenSource.Cancel();
                job.JobStoppingTask = Task.Run(() => { job.JobInstance.StopAsync(job.StopCancellationTokenSource.Token); }, CancellationToken.None); 
            }

            return Task.CompletedTask;
        }
    }
}
