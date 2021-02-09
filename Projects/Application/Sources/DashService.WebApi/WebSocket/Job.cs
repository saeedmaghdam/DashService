using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using DashService.Framework;

namespace DashService.WebApi.WebSocket
{
    public static class Job
    {
        public static async Task Start(Guid jobViewId)
        {
            var jobStructure = Context.JobContainer.PluginableJobs.Where(x => x.PluggedinAssembly.UniqueId == jobViewId).SingleOrDefault();

            if (jobStructure.JobStatus != JobStatus.Running)
            {
                jobStructure.StartCancellationTokenSource = new CancellationTokenSource();
                jobStructure.StopCancellationTokenSource = new CancellationTokenSource();
                jobStructure.JobStartingTask = Task.Run(() => { jobStructure.PluggedinAssembly.JobInstance.StartAsync(jobStructure.StartCancellationTokenSource.Token); }, CancellationToken.None);
                Socket.CallClientMethod(@"
{
    ""command"": ""change_status"",
    ""data"": {
        ""view_id"": """ + jobStructure.PluggedinAssembly.UniqueId + @""",
        ""status"": """ + TaskStatus.Running.ToString() + @"""
    }
}
");
                jobStructure.JobStatus = JobStatus.Running;
            }
        }

        public static async Task Stop(Guid jobViewId)
        {
            var jobStructure = Context.JobContainer.PluginableJobs.Where(x => x.PluggedinAssembly.UniqueId == jobViewId).SingleOrDefault();

            if (jobStructure.JobStatus == JobStatus.Running || jobStructure.JobStatus == JobStatus.Paused)
            {
                jobStructure.StartCancellationTokenSource.Cancel();
                jobStructure.JobStoppingTask = Task.Run(() => { jobStructure.PluggedinAssembly.JobInstance.StopAsync(jobStructure.StopCancellationTokenSource.Token); }, CancellationToken.None);
                Socket.CallClientMethod(@"
{
    ""command"": ""change_status"",
    ""data"": {
        ""view_id"": """ + jobStructure.PluggedinAssembly.UniqueId + @""",
        ""status"": """ + TaskStatus.Canceled.ToString() + @"""
    }
}
");
                jobStructure.JobStatus = JobStatus.Stopped;
            }
        }
    }
}
