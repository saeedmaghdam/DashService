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

            if (jobInstance.StartAsync(Context.Common.CancellationToken).Result)
            {
                Socket.CallClientMethod(@"
{
    ""command"": ""change_status"",
    ""data"": {
        ""view_id"": """ + jobInstance.JobAssembly.UniqueId + @""",
        ""status"": """ + TaskStatus.Running.ToString() + @"""
    }
}
");
            }
        }

        public static async Task Stop(Guid jobViewId)
        {
            var autofacContainer = new Context.CustomDIContainer();
            var jobContainer = autofacContainer.AutofacContainer.Resolve<IJobContainer>();

            var jobInstance = jobContainer.JobInstances.Where(x => x.JobAssembly.UniqueId == jobViewId).SingleOrDefault();

            if (jobInstance.StopAsync(Context.Common.CancellationToken).Result)
            {
                Socket.CallClientMethod(@"
{
    ""command"": ""change_status"",
    ""data"": {
        ""view_id"": """ + jobInstance.JobAssembly.UniqueId + @""",
        ""status"": """ + TaskStatus.Canceled.ToString() + @"""
    }
}
");
            }
        }
    }
}
