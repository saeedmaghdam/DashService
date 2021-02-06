namespace DashService.Logger
{
    public interface ILogger
    {
        void Information(string messageTemplate);
        void Debug(string messageTemplate);
        void Verbose(string messageTemplate);
        void Warning(string messageTemplate);
        void Error(string messageTemplate);
        void Fatal(string messageTemplate);
    }
}
