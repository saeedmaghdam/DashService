using System;
using System.Reflection;

namespace DashService.Framework
{
    public interface IPluggedinAssemblyModel
    {
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
    }
}
