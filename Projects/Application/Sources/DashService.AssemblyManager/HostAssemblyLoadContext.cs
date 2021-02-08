using System;
using System.Linq;
using System.Reflection;
using System.Runtime.Loader;

namespace DashService.JobHandler
{
    public class HostAssemblyLoadContext : Framework.HostAssemblyLoadContext
    {
        private AssemblyDependencyResolver _resolver;

        public HostAssemblyLoadContext(string pluginPath)
        {
            _resolver = new AssemblyDependencyResolver(pluginPath);
        }

        protected override Assembly Load(AssemblyName name)
        {
            string assemblyPath = _resolver.ResolveAssemblyToPath(name);

            if (AppDomain.CurrentDomain.GetAssemblies().Select(x => x.GetName().FullName).Contains(name.FullName))
                return null;

            if (assemblyPath != null)
            {
                Console.WriteLine($"Loading assembly {assemblyPath} into the HostAssemblyLoadContext");
                return LoadFromAssemblyPath(assemblyPath);
            }

            return null;
        }
    }
}
