using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text.RegularExpressions;
using Autofac;
using Autofac.Extensions.DependencyInjection;
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
                .UseServiceProviderFactory(new AutofacServiceProviderFactory())
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
            Logger.DependencyRegistration.RegisterModules(builder);
            RegisterJobs(builder);
        }

        private static void RegisterJobs(ContainerBuilder builder)
        {
            string[] assemblyScannerPattern = new[] { @"DashService.Job.*.dll" };

            Directory.SetCurrentDirectory(AppDomain.CurrentDomain.BaseDirectory);

            List<Assembly> assemblies = new List<Assembly>();
            assemblies.AddRange(
                Directory.EnumerateFiles(Directory.GetCurrentDirectory(), "*.dll", SearchOption.AllDirectories)
                    .Where(filename => assemblyScannerPattern.Any(pattern =>
                        Regex.IsMatch(filename, pattern)
                        && !Regex.IsMatch(filename, "DashService.Job.dll")
                        && !Regex.IsMatch(filename, "DashService.Job.Abstraction.dll")
                    ))
                    .Select(Assembly.LoadFrom)
            );

            foreach (var assembly in assemblies)
            {
                builder.RegisterAssemblyTypes(assembly)
                    .AsImplementedInterfaces();
            }
        }
    }
}
