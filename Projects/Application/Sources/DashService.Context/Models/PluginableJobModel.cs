using System;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using DashService.Framework;
using DashService.Job.Abstraction;

namespace DashService.Context.Models
{
    public class PluginableJobModel : IPluginableJobModel
    {
        public IJob JobInstance
        {
            get;
            set;
        }

        public CancellationTokenSource StartCancellationTokenSource
        {
            get;
            set;
        }

        public CancellationTokenSource StopCancellationTokenSource
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

        public Assembly Assembly
        {
            get;
            set;
        }

        public HostAssemblyLoadContext HostAssemblyLoadContext
        {
            get;
            set;
        }

        public WeakReference WeakReference
        {
            get;
            set;
        }

        public PluginableJobModel(IJob jobInstance, Assembly assembly, HostAssemblyLoadContext hostAssemblyLoadContext, WeakReference weakReference)
        {
            JobInstance = jobInstance;
            Assembly = assembly;
            HostAssemblyLoadContext = hostAssemblyLoadContext;
            WeakReference = weakReference;
            StartCancellationTokenSource = new CancellationTokenSource();
            StopCancellationTokenSource = new CancellationTokenSource();
            JobStatus = JobStatus.None;
        }
    }
}
