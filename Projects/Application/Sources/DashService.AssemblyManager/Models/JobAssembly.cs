using DashService.Framework;
using System;
using System.Reflection;
using DashService.Job.Abstraction;

namespace DashService.JobHandler.Models
{
    public class JobAssembly : IJobAssembly
    {
        public IJob Instance
        {
            get;
            set;
        }

        public Guid UniqueId
        {
            get;
            set;
        }

        public Assembly Assembly
        {
            get;
            set;
        }

        public Framework.HostAssemblyLoadContext HostAssemblyLoadContext
        {
            get;
            set;
        }

        public WeakReference WeakReference
        {
            get;
            set;
        }

        public string JobFullPath
        {
            get;
            set;
        }

        public string FileMD5Hash
        {
            get;
            set;
        }
    }
}
