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
                jobStructure.JobStartingTask = Task.Run(() =>
                {
                    try
                    {
                        jobStructure.JobInstance.StartAsync(jobCancellationTokenSource.Token).Wait(jobCancellationTokenSource.Token);
                    }
                    catch (Exception ex) { }
                }, jobCancellationTokenSource.Token);
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

            foreach (var jobStructure in Context.JobContainer.Jobs.ToList())
            {
                jobStructure.StartCancellationTokenSource.Cancel();
                var jobCancellationTokenSource = jobStructure.StopCancellationTokenSource;
                jobStructure.JobStoppingTask = Task.Run(() =>
                {
                    try
                    {
                        jobStructure.JobInstance.StopAsync(jobCancellationTokenSource.Token).Wait(jobCancellationTokenSource.Token);
                    }
                    catch (Exception ex) { }
                }, jobCancellationTokenSource.Token);
                jobStructure.JobStatus = JobStatus.Stopped;
            }

            Task.WaitAll(Context.JobContainer.Jobs.Select(x=>x.JobStoppingTask).ToArray());

            _logger.Information($"DashService stopped at: {DateTimeOffset.Now}");

            return Task.CompletedTask;
        }
    }
}
