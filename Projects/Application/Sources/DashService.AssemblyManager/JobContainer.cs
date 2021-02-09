using System;
using System.Collections.Generic;
using System.IO;
using DashService.Framework;
using DashService.JobHandler.Models;

namespace DashService.JobHandler
{
    public class JobContainer : IJobContainer
    {
        private readonly IJobLoader _jobLoader;

        private static List<IJobInstance> _jobInstances = new List<IJobInstance>();

        public JobContainer(IJobLoader jobLoader)
        {
            _jobLoader = jobLoader;
        }

        public IEnumerable<IJobInstance> JobInstances => _jobInstances;

        public void Load(string jobPath)
        {
            var jobAssembly = _jobLoader.Load(jobPath);

            Register(new JobInstance(jobAssembly));
        }

        public void LoadDirectory(string jobsPath)
        {
            if (!Directory.Exists(jobsPath))
                throw new Exception("Jobs folder does not exists in bin directory");

            foreach (var directory in Directory.GetDirectories(jobsPath))
                Load(directory);
        }

        private void Register(IJobInstance jobInstance)
        {
            _jobInstances.Add(jobInstance);
        }
    }
}
