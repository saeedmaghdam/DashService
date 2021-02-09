using System.Collections.Generic;

namespace DashService.Framework
{
    public interface IJobContainer
    {
        IEnumerable<IJobInstance> JobInstances
        {
            get;
        }

        void Load(string jobPath);

        void LoadDirectory(string jobsPath);
    }
}
