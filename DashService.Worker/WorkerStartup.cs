using System;
using System.Threading.Tasks;
using System.Threading;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace DashService.Worker
{
    public class WorkerStartup : BackgroundService
    {
        private readonly ILogger<WorkerStartup> _logger;

        public WorkerStartup(ILogger<WorkerStartup> logger)
        {
            _logger = logger;
        }

        public override async Task StartAsync(CancellationToken cancellationToken)
        {
            _logger.LogInformation("DashService started at: {time}", DateTimeOffset.Now);
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
            _logger.LogInformation("DashService stopped at: {time}", DateTimeOffset.Now);

            await Task.CompletedTask;
        }
    }
}
