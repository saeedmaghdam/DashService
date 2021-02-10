using Autofac;
using Microsoft.Extensions.Logging;

namespace DashService.Runtime
{
    public class Startup
    {
        public static void Configure(ContainerBuilder builder)
        {
            builder.RegisterType<Logger.CustomLogger>().As<ILogger>();

            builder.RegisterType<Context.CustomDIContainer>().As<Framework.ICustomDIContainer>();

            builder.RegisterType<JobHandler.JobLoader>().As<Framework.IJobLoader>();

            builder.RegisterType<JobHandler.JobContainer>().As<Framework.IJobContainer>();

            builder.RegisterType<Utils.FileHelper>().As<Framework.Utils.IFileHelper>();
        }
    }
}
