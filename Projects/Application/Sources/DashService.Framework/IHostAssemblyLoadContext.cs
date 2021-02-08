using System.Runtime.Loader;

namespace DashService.Framework
{
    public abstract class HostAssemblyLoadContext : AssemblyLoadContext
    {
        public HostAssemblyLoadContext() : base(isCollectible: true)
        {

        }
    }
}
