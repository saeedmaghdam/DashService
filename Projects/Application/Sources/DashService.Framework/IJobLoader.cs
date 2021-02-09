namespace DashService.Framework
{
    public interface IPluggableJobManager
    {
        IJobInstance Load(string jobsPath);

        void LoadDirectory(string pluginsPath);

        void Unload(IJobInstance pluginableJobInstance);
    }
}
