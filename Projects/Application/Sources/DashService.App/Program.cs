using Autofac;
using DashService.Logger;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace DashService.App
{
    class Program
    {
        static void Main(string[] args)
        {
            CreateHostBuilder(args).Build().Run();
        }

        private static IHostBuilder CreateHostBuilder(string[] args)
        {
            return Host.CreateDefaultBuilder(args)
                .UseWindowsService()
                .UseServiceProviderFactory(new CustomAutofacServiceProviderFactory())
                .ConfigureLogging((hostingContext, logging) =>
                {
                    logging.ClearProviders();
                    logging.AddProvider(new CustomLoggingProvider());
                })
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder.UseStartup<WebApi.HostStartup>();
                })
                .ConfigureServices((hostBuilderContext, services) =>
                {
                    services.AddHostedService<Worker.WorkerStartup>();
                })
                .ConfigureContainer<ContainerBuilder>(ConfigureContainer);
        }

        public static void ConfigureContainer(ContainerBuilder builder)
        {
            Runtime.Startup.Configure(builder);
        }
    }
}
