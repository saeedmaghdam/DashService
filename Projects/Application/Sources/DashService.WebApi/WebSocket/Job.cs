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
                job.StartCancellationTokenSource = new CancellationTokenSource();
                job.StopCancellationTokenSource = new CancellationTokenSource();
                job.JobStartingTask = Task.Run(() => { job.JobInstance.StartAsync(job.StartCancellationTokenSource.Token); }, CancellationToken.None);
                Socket.CallClientMethod(@"
{
    ""command"": ""change_status"",
    ""data"": {
        ""view_id"": """ + job.JobInstance.GetType().GUID + @""",
        ""status"": """ + TaskStatus.Running.ToString() + @"""
    }
}
");
            }

            return Task.CompletedTask;
        }

        public static Task Stop(Guid jobViewId)
        {
            var job = Context.JobContainer.Jobs.Where(x => x.JobInstance.GetType().GUID == jobViewId).SingleOrDefault();

            if (job == null)
                return Task.CompletedTask;

            if (job.JobStartingTask.Status == TaskStatus.Running || job.JobStartingTask.Status == TaskStatus.WaitingForActivation || job.JobStartingTask.Status == TaskStatus.WaitingForChildrenToComplete || job.JobStartingTask.Status == TaskStatus.WaitingToRun)
            {
                job.StartCancellationTokenSource.Cancel();
                job.JobStoppingTask = Task.Run(() => { job.JobInstance.StopAsync(job.StopCancellationTokenSource.Token); }, CancellationToken.None);
                Socket.CallClientMethod(@"
{
    ""command"": ""change_status"",
    ""data"": {
        ""view_id"": """ + job.JobInstance.GetType().GUID + @""",
        ""status"": """ + TaskStatus.Canceled.ToString() + @"""
    }
}
");
            }

            return Task.CompletedTask;
        }
    }
}
