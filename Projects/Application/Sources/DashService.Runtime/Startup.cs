﻿using Autofac;

namespace DashService.Runtime
{
    public class Startup
    {
        public static void Configure(ContainerBuilder builder)
        {
            Logger.DependencyRegistration.RegisterModules(builder);

            builder.RegisterType<Context.DI.CustomContainer>().As<Framework.ICustomContainer>();

            builder.RegisterType<JobHandler.PluggableJobManager>().As<Framework.IPluggableJobManager>();

            builder.RegisterType<Context.JobContainer>().As<Framework.IJobContainer>();
        }
    }
}
