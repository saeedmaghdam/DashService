using System.Collections.Generic;
using DashService.Framework;

namespace DashService.JobHandler
{
    public class JobContainer : IJobContainer
    {
        private static List<IJobInstance> _jobInstances = new List<IJobInstance>();

        public void Register(IEnumerable<IJobInstance> JobInstances)
        {
            foreach (var jobInstance in JobInstances)
            {
                Register(jobInstance);
            }
        }

        public void Register(IJobInstance jobInstance)
        {
            _jobInstances.Add(jobInstance);
        }

        public IEnumerable<IJobInstance> JobInstances => _jobInstances;
    }
}
