using Microsoft.Extensions.Logging;

namespace DashService.Logger
{
    public class CustomLoggingProvider : ILoggerProvider
    {
        private static ILogger _logger;
        private static object _locker = new object();

        public ILogger CreateLogger(string categoryName)
        {
            if (_logger == null)
            {
                lock (_locker)
                {
                    if (_logger == null)
                    {
                        _logger = new CustomLogger();
                    }
                }
            }

            return _logger;
        }

        public void Dispose()
        {
            _logger = null;
        }
    }
}
