using System.Threading;
using System.Threading.Tasks;
using Autofac;
using DashService.Job.Abstraction;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace DashService.Job.Sample
{
    public class Job : JobBase, IJob
    {
        private readonly ILogger _logger;
        private readonly ConfigurationRoot _config;

        public Job(ILogger logger, ConfigurationRoot config) : base(logger, config)
        {
            _logger = logger;
            _logger.BeginScope(this);
            _config = config;
        }

        public Task StartAsync(CancellationToken cancellationToken)
        {
            do
            {
                if (cancellationToken.IsCancellationRequested)
                    break;

                //_logger.LogInformation(new EventId(0, "Service1"), "*************************************");
                _logger.LogInformation(new EventId(0, "Service1"), $"##################{_config.Get<JobOptions>()?.Done}###################");

                Task.Delay(3000, cancellationToken).Wait(cancellationToken);
            }
            while (true);

            return Task.CompletedTask;
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            _logger.LogError("I'm sample job 1! I've finished my task, thank you.");

            return Task.CompletedTask;
        }

        public static void Register(ContainerBuilder builder)
        {
            
        }
    }
}
