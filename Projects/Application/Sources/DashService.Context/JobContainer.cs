using System.Collections.Generic;
using DashService.Framework;

namespace DashService.Context
{
    public static class JobContainer
    {
        private static List<IJobStructure> _pluginableJobs = new List<IJobStructure>();

        public static void Register(IEnumerable<IJobStructure> pluginableJobs)
        {
            foreach (var pluginableJob in pluginableJobs)
            {
                Register(pluginableJob);
            }
        }

        public static void Register(IJobStructure pluginableJob)
        {
            _pluginableJobs.Add(pluginableJob);
        }

        public static IEnumerable<IJobStructure> PluginableJobs => _pluginableJobs;
    }
}
