using Serilog;

namespace DashService.Logger
{
    public class Logger : ILogger
    {
        private readonly Serilog.Core.Logger _log;

        public Logger()
        {
            _log = new LoggerConfiguration()
                .WriteTo.Console()
                .CreateLogger();
        }

        public void Debug(string messageTemplate)
        {
            _log.Debug(messageTemplate);
        }

        public void Error(string messageTemplate)
        {
            _log.Error(messageTemplate);
        }

        public void Fatal(string messageTemplate)
        {
            _log.Fatal(messageTemplate);
        }

        public void Information(string messageTemplate)
        {
            _log.Information(messageTemplate);
        }

        public void Verbose(string messageTemplate)
        {
            _log.Verbose(messageTemplate);
        }

        public void Warning(string messageTemplate)
        {
            _log.Warning(messageTemplate);
        }
    }
}
