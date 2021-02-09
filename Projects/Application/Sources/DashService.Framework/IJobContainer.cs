using System.Collections.Generic;

namespace DashService.Framework
{
    public interface IJobContainer
    {
        IEnumerable<IJobInstance> JobInstances
        {
            get;
        }

        void Register(IEnumerable<IJobInstance> JobInstances);

        void Register(IJobInstance jobInstance);
    }
}
