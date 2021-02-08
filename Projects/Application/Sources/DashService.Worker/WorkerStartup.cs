using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Threading;
using DashService.Framework;
using DashService.JobHandler;
using Microsoft.Extensions.Hosting;
using DashService.Logger;

namespace DashService.Worker
{
    public class WorkerStartup : IHostedService
    {
        private readonly ILogger _logger;

        public WorkerStartup(ILogger logger)
        {
            _logger = logger;

            var pluginsPath = Path.Combine(Path.GetDirectoryName(System.Reflection.Assembly.GetEntryAssembly().Location), "Jobs");
            PluggableJobManager.LoadDirectory(pluginsPath);
        }

        public async Task StartAsync(CancellationToken cancellationToken)
        {
            _logger.Information($"DashService was starting the micro services at: {DateTimeOffset.Now}");

            foreach (var jobStructure in Context.JobContainer.PluginableJobs.ToList())
            {
                var jobCancellationTokenSource = jobStructure.StartCancellationTokenSource;
                jobStructure.JobStartingTask = Task.Run(() => { jobStructure.JobInstance.StartAsync(jobStructure.StartCancellationTokenSource.Token); }, jobCancellationTokenSource.Token);
                jobStructure.JobStatus = JobStatus.Running;
            }

            _logger.Information($"DashService started at: {DateTimeOffset.Now}");

            do
            {
                if (cancellationToken.IsCancellationRequested)
                    break;

                try
                {
                    Task.Delay(60000, cancellationToken).Wait();
                }
                catch (Exception) { }
            }
            while (true);
        }

        public async Task StopAsync(CancellationToken cancellationToken)
        {
            _logger.Information($"DashService was stopping the micro services at: {DateTimeOffset.Now}");

            foreach (var jobStructure in Context.JobContainer.PluginableJobs.ToList())
            {
                if (jobStructure.JobStatus == JobStatus.Running || jobStructure.JobStatus == JobStatus.Paused)
                {
                    jobStructure.StartCancellationTokenSource.Cancel();
                    var jobCancellationTokenSource = jobStructure.StopCancellationTokenSource;
                    jobStructure.JobStoppingTask = Task.Run(() => { jobStructure.JobInstance.StopAsync(jobStructure.StopCancellationTokenSource.Token); }, CancellationToken.None);
                }
            }

            Task.WaitAll(Context.JobContainer.PluginableJobs.Select(x => x.JobStoppingTask).ToArray());

            _logger.Information($"DashService stopped at: {DateTimeOffset.Now}");
        }
    }
}
