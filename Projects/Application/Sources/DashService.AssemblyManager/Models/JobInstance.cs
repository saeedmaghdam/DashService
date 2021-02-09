using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using DashService.Framework;
using DashService.Framework.Utils;
using DashService.Logger;

namespace DashService.JobHandler.Models
{
    public class JobInstance : IJobInstance
    {
        private FileSystemWatcher _fileSystemWatcher;
        private IJobLoader _jobLoader;
        private ILogger _logger;

        public CancellationTokenSource JobLoadCancellationTokenSource
        {
            get;
            set;
        }

        public CancellationTokenSource JobUnloadCancellationTokenSource
        {
            get;
            set;
        }

        public Task JobStartingTask
        {
            get;
            set;
        }

        public Task JobStoppingTask
        {
            get;
            set;
        }

        public JobStatus JobStatus
        {
            get;
            set;
        }
        public IJobAssembly JobAssembly
        {
            get;
            set;
        }

        public bool UpdatingMode
        {
            get;
            set;
        }

        public JobInstance(IJobAssembly jobAssembly, IJobLoader jobLoader, ILogger logger)
        {
            _jobLoader = jobLoader;
            _logger = logger;

            JobAssembly = jobAssembly;
            JobLoadCancellationTokenSource = new CancellationTokenSource();
            JobUnloadCancellationTokenSource = new CancellationTokenSource();
            JobStatus = JobStatus.None;

            WatchJob();
        }

        private void WatchJob()
        {
            _fileSystemWatcher = new FileSystemWatcher
            {
                Filter = Path.GetFileName(JobAssembly.JobFullPath),
                Path = Path.GetDirectoryName(JobAssembly.JobFullPath),
                IncludeSubdirectories = false,
                EnableRaisingEvents = true
            };

            _fileSystemWatcher.Changed += (sender, args) =>
            {
                if (UpdatingMode)
                    return;

                UpdatingMode = true;

                JobLoadCancellationTokenSource.Cancel();
            };
        }

        public async Task<Task> StartAsync(CancellationToken cancellationToken)
        {
            return Task.Run(() =>
            {
                do
                {
                    try
                    {
                        JobAssembly.Instance.StartAsync(JobLoadCancellationTokenSource.Token).Wait(JobLoadCancellationTokenSource.Token);
                    }
                    catch { }

                    JobStatus = JobStatus.Stopped;

                    if (UpdatingMode)
                    {
                        // Stop job if already working
                        if (JobStatus == JobStatus.Running || JobStatus == JobStatus.Paused)
                            JobAssembly.Instance.StopAsync(JobUnloadCancellationTokenSource.Token).Wait(JobUnloadCancellationTokenSource.Token);

                        // Unload current job
                        JobAssembly.HostAssemblyLoadContext.Unload();

                        // Try to load new assembly
                        int retryWaitingTimeInMs = 2000;
                        int retryCounter = 0;
                        do
                        {
                            try
                            {
                                var jobAssembly = _jobLoader.Load(Path.GetDirectoryName(JobAssembly.JobFullPath));

                                if (jobAssembly != null)
                                {
                                    JobAssembly = jobAssembly;
                                    JobLoadCancellationTokenSource = new CancellationTokenSource();
                                    JobUnloadCancellationTokenSource = new CancellationTokenSource();
                                    JobStatus = JobStatus.None;

                                    break;
                                }
                            }
                            catch (Exception ex)
                            {
                                _logger.Error("Couldn't load the job!");

                                if (retryCounter <= 60)
                                {
                                    switch (retryCounter)
                                    {
                                        case 20:
                                            retryWaitingTimeInMs = 10000;
                                            break;
                                        case 40:
                                            retryWaitingTimeInMs = 20000;
                                            break;
                                        case 60:
                                            retryWaitingTimeInMs = 60000;
                                            break;
                                    }

                                    retryCounter++;
                                }

                                try
                                {
                                    Task.Delay(retryWaitingTimeInMs, cancellationToken).Wait(cancellationToken);
                                }
                                catch { }
                            }
                        } while (true);

                        UpdatingMode = false;
                    }
                    else
                    {
                        break;
                    }
                } while (true);
            });
        }

        public async Task<Task> StopAsync(CancellationToken cancellationToken)
        {
            return Task.Run(() =>
            {
                JobAssembly.Instance.StopAsync(JobUnloadCancellationTokenSource.Token);
            }, cancellationToken);
        }
    }
}
