using Microsoft.Extensions.Logging;
using Serilog;
using System;

namespace DashService.Logger
{
    public class CustomLogger : Microsoft.Extensions.Logging.ILogger
    {
        private Serilog.ILogger _logger;
        private readonly IExternalScopeProvider _externalScopeProvider;

        public CustomLogger()
        {
            _externalScopeProvider = new LoggerExternalScopeProvider();

            _logger = new Serilog.LoggerConfiguration()
                .WriteTo.Console()
                .WriteTo.File("log.txt", rollingInterval: RollingInterval.Day)
                .CreateLogger();
        }

        public IDisposable BeginScope<TState>(TState state)
        {
            return _externalScopeProvider?.Push(state) ?? NullScope.Instance;
        }

        public bool IsEnabled(LogLevel logLevel)
        {
            return true;
        }

        public void Log<TState>(LogLevel logLevel, EventId eventId, TState state, Exception exception, Func<TState, Exception, string> formatter)
        {
            if (logLevel == LogLevel.None)
                return;

            Serilog.Events.LogEventLevel logEventLevel = Serilog.Events.LogEventLevel.Information;
            switch (logLevel)
            {
                case LogLevel.Information:
                    logEventLevel = Serilog.Events.LogEventLevel.Information;
                    break;
                case LogLevel.Error:
                    logEventLevel = Serilog.Events.LogEventLevel.Error;
                    break;
                case LogLevel.Warning:
                    logEventLevel = Serilog.Events.LogEventLevel.Warning;
                    break;
                case LogLevel.Critical:
                    logEventLevel = Serilog.Events.LogEventLevel.Fatal;
                    break;
                case LogLevel.Debug:
                    logEventLevel = Serilog.Events.LogEventLevel.Debug;
                    break;
                case LogLevel.Trace:
                    logEventLevel = Serilog.Events.LogEventLevel.Verbose;
                    break;
            }

            _logger.Write(logEventLevel, exception, formatter.Invoke(state, exception));
        }
    }

    public class NullScope : IDisposable
    {
        public static IDisposable Instance { get; } = new NullScope();

        public void Dispose()
        {
        }
    }
}
