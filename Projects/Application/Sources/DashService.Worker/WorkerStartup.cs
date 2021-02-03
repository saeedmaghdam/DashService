using System;
using System.Threading.Tasks;
using System.Threading;
using DashService.Job.Abstraction;
using Microsoft.Extensions.Hosting;
using DashService.Logger;

namespace DashService.Worker
{
    public class WorkerStartup : BackgroundService
    {
        private readonly ILogger _logger;
        private readonly IEnumerable<IJob> _jobs;

        public WorkerStartup(ILogger logger, IEnumerable<IJob> jobs)
        {
            _logger = logger;
            _jobs = jobs;
        }

        public override async Task StartAsync(CancellationToken cancellationToken)
        {
            _logger.Information($"DashService started at: {DateTimeOffset.Now}");
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            await Task.Run(() =>
            {
                do
                {
                    if (stoppingToken.IsCancellationRequested)
                        break;
                } while (true);
            });
        }

        public override async Task StopAsync(CancellationToken cancellationToken)
        {
            _logger.Information($"DashService stopped at: {DateTimeOffset.Now}");

            await Task.CompletedTask;
        }
    }
}
