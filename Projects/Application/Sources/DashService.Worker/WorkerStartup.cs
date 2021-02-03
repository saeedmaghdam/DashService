using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Threading;
using DashService.Job.Abstraction;
using Microsoft.Extensions.Hosting;
using DashService.Logger;
using DashService.Context.Framework;

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

        public Task StartAsync(CancellationToken cancellationToken)
        {
            _logger.Information($"DashService was starting the micro services at: {DateTimeOffset.Now}");

            foreach (var jobStructure in Context.JobContainer.Jobs.ToList())
            {
                var jobCancellationTokenSource = jobStructure.StartCancellationTokenSource;
                Task.Run(() => { jobStructure.JobInstance.StartAsync(jobCancellationTokenSource.Token); }, jobCancellationTokenSource.Token);
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
                catch (Exception ex) { }
            }
            while (true);

            return Task.CompletedTask;
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            _logger.Information($"DashService was stopping the micro services at: {DateTimeOffset.Now}");

            var tasks = new List<Task>();
            foreach (var job in Context.JobContainer.Jobs.ToList())
            {
                job.StartCancellationTokenSource.Cancel();
                var jobCancellationTokenSource = job.StopCancellationTokenSource;
                tasks.Add(Task.Run(() => { job.JobInstance.StopAsync(jobCancellationTokenSource.Token); }, jobCancellationTokenSource.Token));
                job.JobStatus = JobStatus.Stopped;
            }

            Task.WaitAll(tasks.ToArray());

            _logger.Information($"DashService stopped at: {DateTimeOffset.Now}");

            return Task.CompletedTask;
        }
    }
}
