using DashService.Logger;
using System.Threading;
using System.Threading.Tasks;
using DashService.Job.Abstraction;

namespace DashService.Job.Sample
{
    public class Job : JobBase, IJob
    {
        private readonly ILogger _logger;

        public override string Name => "Sample Job";
        public override string Description => "Test Job";
        public override string Version => "1.0.0";

        public Job(ILogger logger) : base(logger)
        {
            _logger = logger;
        }

        public Task StartAsync(CancellationToken cancellationToken)
        {
            _logger.Information("Hello, I'm the sample job!");

            do
            {
                if (cancellationToken.IsCancellationRequested)
                    break;

                _logger.Information("I'm sample job, I'm doing a process ...");

                Task.Delay(3000, cancellationToken).Wait(cancellationToken);
            }
            while (true);

            return Task.CompletedTask;
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            _logger.Information("I'm sample job! I've finished my task, thank you.");

            return Task.CompletedTask;
        }
    }
}
