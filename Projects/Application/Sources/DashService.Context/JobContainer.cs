using System.Collections.Generic;
using DashService.Framework;

namespace DashService.Context
{
    public static class JobContainer
    {
        private static List<IJobInstance> _jobInstances = new List<IJobInstance>();

        public static void Register(IEnumerable<IJobInstance> JobInstances)
        {
            foreach (var jobInstance in JobInstances)
            {
                Register(jobInstance);
            }
        }

        public static void Register(IJobInstance jobInstance)
        {
            _jobInstances.Add(jobInstance);
        }

        public static IEnumerable<IJobInstance> JobInstances => _jobInstances;
    }
}
