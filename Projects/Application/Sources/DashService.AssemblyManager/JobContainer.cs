using System;
using System.Collections.Generic;
using System.IO;
using DashService.Framework;
using DashService.JobHandler.Models;
using DashService.Logger;

namespace DashService.JobHandler
{
    public class JobContainer : IJobContainer
    {
        private readonly IJobLoader _jobLoader;
        private readonly ILogger _logger;


        private static List<IJobInstance> _jobInstances = new List<IJobInstance>();

        public JobContainer(IJobLoader jobLoader, ILogger logger)
        {
            _jobLoader = jobLoader;
            _logger = logger;
        }

        public IEnumerable<IJobInstance> JobInstances => _jobInstances;

        public void Load(string jobPath)
        {
            var jobAssembly = _jobLoader.Load(jobPath);

            Register(new JobInstance(jobAssembly, _jobLoader, _logger));
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
