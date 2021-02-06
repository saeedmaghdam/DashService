using Autofac;

namespace DashService.Logger
{
    public static class DependencyRegistration
    {
        public static void RegisterModules(ContainerBuilder containerBuilder)
        {
            containerBuilder.RegisterType<Logger>().As<ILogger>();
        }
    }
}
