namespace DashService.Framework
{
    public interface IJobLoader
    {
        IJobAssembly Load(string jobsPath);

        void Unload(IJobInstance pluginableJobInstance);
    }
}
