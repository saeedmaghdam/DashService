using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Threading;
using DashService.Framework;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

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

            _logger.LogInformation($"DashService was starting the micro services at: {DateTimeOffset.Now}");

            foreach (var jobInstance in _jobContainer.JobInstances.ToList())
                jobInstance.JobStartingTask = jobInstance.StartAsync(cancellationToken);

            _logger.LogInformation($"DashService started at: {DateTimeOffset.Now}");

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
            _logger.LogInformation($"DashService was stopping the micro services at: {DateTimeOffset.Now}");

            foreach (var jobInstance in _jobContainer.JobInstances.ToList())
                jobInstance.StopAsync(cancellationToken);

            try
            {
                Task.WaitAll(_jobContainer.JobInstances.Select(x => x.JobStoppingTask).ToArray(), cancellationToken);
            }
            catch { }

            _logger.LogInformation($"DashService stopped at: {DateTimeOffset.Now}");
        }
    }
}
