using DashService.Logger;
using System.Threading;
using System.Threading.Tasks;
using Autofac;
using DashService.Job.Abstraction;

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
        }

        public Task StartAsync(CancellationToken cancellationToken)
        {
            do
            {
                if (cancellationToken.IsCancellationRequested)
                    break;

                //_logger.Information("*************************************");
                _logger.Information("$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$");

                Task.Delay(3000, cancellationToken).Wait(cancellationToken);
            }
            while (true);

            return Task.CompletedTask;
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            _logger.Error("I'm sample job 1! I've finished my task, thank you.");

            return Task.CompletedTask;
        }

        public static void Register(ContainerBuilder builder)
        {

        }
    }
}
