using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Threading;
using DashService.Job.Abstraction;
using Microsoft.Extensions.Hosting;
using DashService.Logger;

namespace DashService.Worker
{
    public class WorkerStartup : IHostedService
    {
        private readonly ILogger _logger;
        private readonly IEnumerable<IJob> _jobs;

        public WorkerStartup(ILogger logger, IEnumerable<IJob> jobs)
        {
            _logger = logger;
            _jobs = jobs;
        }

        public Task StartAsync(CancellationToken cancellationToken)
        {
            _logger.Information($"DashService was starting the micro services at: {DateTimeOffset.Now}");

            foreach (var job in _jobs.ToList())
                Task.Run(() => { job.StartAsync(cancellationToken); }, cancellationToken);

            _logger.Information($"DashService started at: {DateTimeOffset.Now}");

            return Task.CompletedTask;
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            _logger.Information($"DashService was stopping the micro services at: {DateTimeOffset.Now}");

            foreach (var job in _jobs.ToList())
                Task.Run(() => { job.StopAsync(cancellationToken); }, cancellationToken);

            _logger.Information($"DashService stopped at: {DateTimeOffset.Now}");

            return Task.CompletedTask;
        }
    }
}
