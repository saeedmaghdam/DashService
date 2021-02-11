using System.Threading;
using System.Threading.Tasks;

namespace DashService.Job.Abstraction
{
    public interface IJob
    {
        string Name
        {
            get;
        }

        string Description
        {
            get;
        }

        Task StartAsync(CancellationToken cancellationToken);

        Task StopAsync(CancellationToken cancellationToken);
    }
}
