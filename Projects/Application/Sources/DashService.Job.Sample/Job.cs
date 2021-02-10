using System.Threading;
using System.Threading.Tasks;
using Autofac;
using DashService.Job.Abstraction;
using Microsoft.Extensions.Logging;

namespace DashService.Job.Sample
{
    public class Job : JobBase, IJob
    {
        private readonly ILogger _logger;

        public override string Name => "Sample Job";
        public override string Description => "Test Job";
        public override string Version => "2.2.2";

        public Job(ILogger logger) : base(logger)
        {
            _logger = logger;
            _logger.BeginScope(this);
        }

        public Task StartAsync(CancellationToken cancellationToken)
        {
            do
            {
                if (cancellationToken.IsCancellationRequested)
                    break;

                //_logger.LogInformation(new EventId(0, "Service1"), "*************************************");
                _logger.LogInformation(new EventId(0, "Service1"), "#####################################");

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
