using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Threading;
using DashService.Job.Abstraction;
using Microsoft.Extensions.Hosting;
using DashService.Logger;
using DashService.Worker.Framework;

namespace DashService.Worker
{
    public class WorkerStartup : IHostedService
    {
        private readonly ILogger _logger;
        private readonly Dictionary<IJob, JobDetails> _jobs;

        public WorkerStartup(ILogger logger, IEnumerable<IJob> jobs)
        {
            _logger = logger;

            _jobs = new Dictionary<IJob, JobDetails>();
            foreach (var job in jobs)
            {
                _jobs.Add(job, new JobDetails()
                {
                    StartCancellationTokenSource = new CancellationTokenSource(),
                    StopCancellationTokenSource = new CancellationTokenSource(),
                    JobStatus = JobStatus.Stopped
                });
            }
        }

        public Task StartAsync(CancellationToken cancellationToken)
        {
            _logger.Information($"DashService was starting the micro services at: {DateTimeOffset.Now}");

            foreach (var job in _jobs)
            {
                var jobCancellationTokenSource = job.Value.StartCancellationTokenSource;
                Task.Run(() => { job.Key.StartAsync(jobCancellationTokenSource.Token); }, jobCancellationTokenSource.Token);
                job.Value.JobStatus = JobStatus.Running;
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
            foreach (var job in _jobs.ToList())
            {
                job.Value.StartCancellationTokenSource.Cancel();
                var jobCancellationTokenSource = job.Value.StopCancellationTokenSource;
                tasks.Add(Task.Run(() => { job.Key.StopAsync(jobCancellationTokenSource.Token); }, jobCancellationTokenSource.Token));
                job.Value.JobStatus = JobStatus.Stopped;
            }

            Task.WaitAll(tasks.ToArray());

            _logger.Information($"DashService stopped at: {DateTimeOffset.Now}");

            return Task.CompletedTask;
        }
    }
}
