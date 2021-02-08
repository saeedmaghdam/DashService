using System.Collections.Generic;
using DashService.Framework;

namespace DashService.Context
{
    public static class JobContainer
    {
        private static List<IPluginableJobModel> _pluginableJobs = new List<IPluginableJobModel>();

        public static void Register(IEnumerable<IPluginableJobModel> pluginableJobs)
        {
            foreach (var pluginableJob in pluginableJobs)
            {
                Register(pluginableJob);
            }
        }

        public static void Register(IPluginableJobModel pluginableJob)
        {
            _pluginableJobs.Add(pluginableJob);
        }

        public static IEnumerable<IPluginableJobModel> PluginableJobs => _pluginableJobs;
    }
}
