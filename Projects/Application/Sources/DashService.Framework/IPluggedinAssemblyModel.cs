using System;
using System.Reflection;

namespace DashService.Framework
{
    public interface IPluggedinAssemblyModel
    {
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
