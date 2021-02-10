using Microsoft.Extensions.Logging;
using System;

namespace DashService.Job
{
    public abstract class JobBase
    {
        private readonly ILogger _logger;

        public ILogger Logger => _logger;

        public virtual string Name => throw new NotImplementedException();

        public virtual string Description => throw new NotImplementedException();

        public virtual string Version => throw new NotImplementedException();

        public JobBase(ILogger logger)
        {
            _logger = logger;
        }
    }
}
