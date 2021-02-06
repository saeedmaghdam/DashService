using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Threading;
using DashService.Context.Models;
using DashService.Job.Abstraction;
using Microsoft.Extensions.Hosting;
using DashService.Logger;

namespace DashService.Worker
{
    public class WorkerStartup : IHostedService
    {
        private readonly ILogger _logger;

        public WorkerStartup(ILogger logger, IEnumerable<IJob> jobs)
        {
            _logger = logger;
            Context.JobContainer.Add(jobs);
        }

        public async Task StartAsync(CancellationToken cancellationToken)
        {
            _logger.Information($"DashService was starting the micro services at: {DateTimeOffset.Now}");

            foreach (var jobStructure in Context.JobContainer.Jobs.ToList())
            {
                var jobCancellationTokenSource = jobStructure.StartCancellationTokenSource;
                jobStructure.JobStartingTask = Task.Run(() => { jobStructure.JobInstance.StartAsync(jobStructure.StartCancellationTokenSource.Token); }, CancellationToken.None);
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

            foreach (var jobStructure in Context.JobContainer.Jobs.ToList())
            {
                if (jobStructure.JobStatus == JobStatus.Running || jobStructure.JobStatus == JobStatus.Paused)
                {
                    jobStructure.StartCancellationTokenSource.Cancel();
                    var jobCancellationTokenSource = jobStructure.StopCancellationTokenSource;
                    jobStructure.JobStoppingTask = Task.Run(() => { jobStructure.JobInstance.StopAsync(jobStructure.StopCancellationTokenSource.Token); }, CancellationToken.None);
                }
            }

            Task.WaitAll(Context.JobContainer.Jobs.Select(x => x.JobStoppingTask).ToArray());

            _logger.Information($"DashService stopped at: {DateTimeOffset.Now}");
        }
    }
}
