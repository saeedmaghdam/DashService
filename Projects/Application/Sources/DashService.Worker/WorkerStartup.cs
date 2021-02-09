using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Threading;
using DashService.Framework;
using Microsoft.Extensions.Hosting;
using DashService.Logger;

namespace DashService.Worker
{
    public class WorkerStartup : IHostedService
    {
        private readonly ILogger _logger;
        private readonly IJobContainer _jobContainer;
        private readonly IJobLoader _jobLoader;

        public WorkerStartup(ILogger logger, IJobContainer jobContainer, IJobLoader jobLoader)
        {
            _logger = logger;
            _jobContainer = jobContainer;
            _jobLoader = jobLoader;

            var pluginsPath = Path.Combine(Path.GetDirectoryName(System.Reflection.Assembly.GetEntryAssembly().Location), "Jobs");
            _jobContainer.LoadDirectory(pluginsPath);
        }

        public async Task StartAsync(CancellationToken cancellationToken)
        {
            Context.Common.CancellationToken = cancellationToken;

            _logger.Information($"DashService was starting the micro services at: {DateTimeOffset.Now}");

            foreach (var jobInstance in _jobContainer.JobInstances.ToList())
            {
                jobInstance.JobStartingTask = jobInstance.StartAsync(cancellationToken);
                jobInstance.JobStatus = JobStatus.Running;
            }

            _logger.Information($"DashService started at: {DateTimeOffset.Now}");

            do
            {
                if (cancellationToken.IsCancellationRequested)
                    break;

                try
                {
                    Task.Delay(60000, cancellationToken).Wait(cancellationToken);
                }
                catch { }
            }
            while (true);
        }

        public async Task StopAsync(CancellationToken cancellationToken)
        {
            _logger.Information($"DashService was stopping the micro services at: {DateTimeOffset.Now}");

            foreach (var jobInstance in _jobContainer.JobInstances.ToList())
            {
                if (jobInstance.JobStatus == JobStatus.Running || jobInstance.JobStatus == JobStatus.Paused)
                {
                    jobInstance.JobLoadCancellationTokenSource.Cancel();
                    jobInstance.JobStoppingTask = jobInstance.StopAsync(cancellationToken);
                }
            }

            try
            {
                Task.WaitAll(_jobContainer.JobInstances.Select(x => x.JobStoppingTask).ToArray(), cancellationToken);
            }
            catch { }

            _logger.Information($"DashService stopped at: {DateTimeOffset.Now}");
        }
    }
}
