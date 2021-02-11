using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace DashService.Job
{
    public abstract class JobBase
    {
        private readonly ILogger _logger;

        public ILogger Logger => _logger;

        public virtual string Name
        {
            get;
            set;
        }

        public virtual string Description
        {
            get;
            set;
        }

        public JobBase(ILogger logger, ConfigurationRoot config)
        {
            _logger = logger;

            var jobOptions = config.Get<JobOptions>();
            Name = jobOptions.JobHeader?.Name ?? "Name not defined";
            Description = jobOptions.JobHeader?.Description ?? "Description not defined";
        }
    }
}
