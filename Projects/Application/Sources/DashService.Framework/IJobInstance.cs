﻿using System.Threading;
using System.Threading.Tasks;

namespace DashService.Framework
{
    public interface IJobInstance
    {
        CancellationTokenSource JobLoadCancellationTokenSource
        {
            get;
            set;
        }

        CancellationTokenSource JobUnloadCancellationTokenSource
        {
            get;
            set;
        }

        Task JobStartingTask
        {
            get;
            set;
        }

        Task JobStoppingTask
        {
            get;
            set;
        }

        JobStatus JobStatus
        {
            get;
            set;
        }

        IJobAssembly JobAssembly
        {
            get;
            set;
        }

        bool UpdatingMode
        {
            get;
            set;
        }

        Task<bool> StartAsync(CancellationToken cancellationToken);

        Task<bool> StopAsync(CancellationToken cancellationToken);
    }
}
