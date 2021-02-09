using System;
using System.Reflection;
using DashService.Job.Abstraction;

namespace DashService.Framework
{
    public interface IPluggedinAssemblyModel
    {
        IJob JobInstance
        {
            get;
            set;
        }
        Guid UniqueId
        {
            get;
            set;
        }

        Assembly Assembly
        {
            get;
            set;
        }

        HostAssemblyLoadContext HostAssemblyLoadContext
        {
            get;
            set;
        }

        WeakReference WeakReference
        {
            get;
            set;
        }

        string JobFullPath
        {
            get;
            set;
        }
    }
}
