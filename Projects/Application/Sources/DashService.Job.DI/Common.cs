using System.Threading;

namespace DashService.Context
{
    public class Common
    {
        private static CancellationToken _cancellationToken;

        public static CancellationToken CancellationToken {
            get => _cancellationToken;
            set
            {
                if (_cancellationToken == null)
                    _cancellationToken = value;
            } 
        }
    }
}
