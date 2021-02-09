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

            foreach (var jobInstance in Context.JobContainer.JobInstances.ToList())
            {
                var jobCancellationTokenSource = jobInstance.StartCancellationTokenSource;
                jobInstance.JobStartingTask = Task.Run(() => { jobInstance.JobAssembly.Instance.StartAsync(jobInstance.StartCancellationTokenSource.Token); }, jobCancellationTokenSource.Token);
                jobInstance.JobStatus = JobStatus.Running;
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

            foreach (var jobInstance in Context.JobContainer.JobInstances.ToList())
            {
                if (jobInstance.JobStatus == JobStatus.Running || jobInstance.JobStatus == JobStatus.Paused)
                {
                    jobInstance.StartCancellationTokenSource.Cancel();
                    var jobCancellationTokenSource = jobInstance.StopCancellationTokenSource;
                    jobInstance.JobStoppingTask = Task.Run(() => { jobInstance.JobAssembly.Instance.StopAsync(jobInstance.StopCancellationTokenSource.Token); }, CancellationToken.None);
                }
            }

            Task.WaitAll(Context.JobContainer.JobInstances.Select(x => x.JobStoppingTask).ToArray());

            _logger.Information($"DashService stopped at: {DateTimeOffset.Now}");
        }
    }
}
