using Autofac;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

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
                .ConfigureContainer<ContainerBuilder>(ConfigureContainer)
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder.UseStartup<WebApi.HostStartup>();
                })
                .ConfigureServices((hostBuilderContext, services) =>
                {
                    services.AddHostedService<Worker.WorkerStartup>();
                });
        }

        public static void ConfigureContainer(ContainerBuilder builder)
        {
            Runtime.Startup.Configure(builder);
        }
    }
}
