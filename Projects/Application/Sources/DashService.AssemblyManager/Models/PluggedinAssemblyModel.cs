using DashService.Framework;
using System;
using System.Reflection;

namespace DashService.JobHandler.Models
{
    public class PluggedinAssemblyModel : IPluggedinAssemblyModel
    {
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
    }
}
