using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Autofac;
using DashService.Framework;

namespace DashService.WebApi.WebSocket
{
    public class Job
    {
        public static async Task Start(Guid jobViewId)
        {
            var autofacContainer = new Context.CustomDIContainer();
            var jobContainer = autofacContainer.AutofacContainer.Resolve<IJobContainer>();

            var jobInstance = jobContainer.JobInstances.Where(x => x.JobAssembly.UniqueId == jobViewId).SingleOrDefault();

            if (jobInstance.JobStatus != JobStatus.Running)
            {
                jobInstance.JobLoadCancellationTokenSource = new CancellationTokenSource();
                jobInstance.JobUnloadCancellationTokenSource = new CancellationTokenSource();
                jobInstance.JobStartingTask = jobInstance.StartAsync(CancellationToken.None);
                Socket.CallClientMethod(@"
{
    ""command"": ""change_status"",
    ""data"": {
        ""view_id"": """ + jobInstance.JobAssembly.UniqueId + @""",
        ""status"": """ + TaskStatus.Running.ToString() + @"""
    }
}
");
                jobInstance.JobStatus = JobStatus.Running;
            }
        }

        public static async Task Stop(Guid jobViewId)
        {
            var autofacContainer = new Context.CustomDIContainer();
            var jobContainer = autofacContainer.AutofacContainer.Resolve<IJobContainer>();

            var jobInstance = jobContainer.JobInstances.Where(x => x.JobAssembly.UniqueId == jobViewId).SingleOrDefault();

            if (jobInstance.JobStatus == JobStatus.Running || jobInstance.JobStatus == JobStatus.Paused)
            {
                jobInstance.JobLoadCancellationTokenSource.Cancel();
                jobInstance.JobStoppingTask = jobInstance.StopAsync(CancellationToken.None);
                Socket.CallClientMethod(@"
{
    ""command"": ""change_status"",
    ""data"": {
        ""view_id"": """ + jobInstance.JobAssembly.UniqueId + @""",
        ""status"": """ + TaskStatus.Canceled.ToString() + @"""
    }
}
");
                jobInstance.JobStatus = JobStatus.Stopped;
            }
        }
    }
}
