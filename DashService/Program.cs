using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System.IO;

namespace DashService
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
                .UseSystemd()
                .UseContentRoot(Directory.GetCurrentDirectory())
                .ConfigureServices((hostBuilderContext, services) =>
                {
                    services.AddHostedService<Worker.WorkerStartup>();
                })
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder.UseStartup<WebApi.HostStartup>();
                });
        }
    }
}
