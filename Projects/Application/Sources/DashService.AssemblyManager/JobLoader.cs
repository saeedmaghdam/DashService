﻿using System;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using Autofac;
using DashService.Framework;
using DashService.Framework.Utils;
using DashService.Job.Abstraction;
using DashService.JobHandler.Models;
using Microsoft.Extensions.Configuration;

namespace DashService.JobHandler
{
    public class JobLoader : IJobLoader
    {
        private readonly ICustomDIContainer _customContainer;
        private readonly IFileHelper _fileHelper;

        public JobLoader(ICustomDIContainer customContainer, IFileHelper fileHelper)
        {
            _customContainer = customContainer;
            _fileHelper = fileHelper;
        }

        [MethodImpl(MethodImplOptions.NoInlining)]
        public IJobAssembly Load(string jobsPath)
        {
            var dllFiles = Directory.GetFiles(jobsPath, "*.dll");

            var jobFilePath = "";
            foreach (var dllFile in dllFiles)
            {
                var tempAlc = new HostAssemblyLoadContext(dllFile);
                
                Assembly tempAssembly;
                using (var stream = File.OpenRead(dllFile))
                {
                    tempAssembly = tempAlc.LoadFromStream((Stream)stream);
                }

                var types = tempAssembly.GetTypes();
                var jobClass = types.SingleOrDefault(x => x.GetInterfaces().Contains(typeof(IJob)));

                tempAlc.Unload();

                if (jobClass != null)
                {
                    jobFilePath = dllFile;
                    break;
                }
            }

            if (string.IsNullOrEmpty(jobFilePath))
                return null;

            var alc = new HostAssemblyLoadContext(jobFilePath);
            var alcWeakRef = new WeakReference(alc);

            Assembly assembly;
            using (var stream = File.OpenRead(jobFilePath))
            {
                assembly = alc.LoadFromStream((Stream)stream);
            }

            var jobType = assembly.GetTypes().SingleOrDefault(x => x.GetInterfaces().Contains(typeof(IJob)));
            if (jobType == null)
                return null;

            var containerBuilder = new ContainerBuilder();

            var configurationBuilder = new ConfigurationBuilder()
                .SetBasePath(jobsPath)
                .AddJsonFile($"appsettings.json", true, true)
                .AddEnvironmentVariables();
            foreach (var appSettingFile in Directory.GetFiles(jobsPath, "appsettings.*.json"))
                configurationBuilder.AddJsonFile(Path.GetFileName(appSettingFile));
            var config = configurationBuilder.Build();
            containerBuilder.RegisterInstance((ConfigurationRoot)config);

            MethodInfo methodInfo = jobType.GetMethod("Register");
            if (methodInfo == null)
                return null;

            methodInfo.Invoke(null, new object[] { containerBuilder });
            var container = containerBuilder.Build();
            var jobInstance = (IJob)Instance(jobType, container);

            var jobAssembly = new JobAssembly()
            {
                Instance = jobInstance,
                UniqueId = Guid.NewGuid(),
                Assembly = assembly,
                HostAssemblyLoadContext = alc,
                WeakReference = alcWeakRef,
                JobFullPath = jobFilePath,
                FileMD5Hash = _fileHelper.CalculateMD5(jobFilePath)
            };

            return jobAssembly;
        }

        [MethodImpl(MethodImplOptions.NoInlining)]
        public void Unload(IJobInstance pluginableJobInstance)
        {
            pluginableJobInstance.JobAssembly.HostAssemblyLoadContext.Unload();
        }

        [MethodImpl(MethodImplOptions.NoInlining)]
        private object Instance(Type jobType, IContainer container)
        {
            Type instanceType = jobType;
            var constructorInfos = instanceType.GetConstructors();
            ConstructorInfo constructorInfo = constructorInfos[0];
            ParameterInfo[] constructorParamsInfo = constructorInfo.GetParameters();
            object[] constructorParams = new object[constructorParamsInfo.Length];

            for (int i = 0; i < constructorParamsInfo.Length; i++)
            {
                var parameterInfo = constructorParamsInfo[i];
                var parameterType = parameterInfo.ParameterType;
                if (_customContainer.AutofacContainer.IsRegistered(parameterType))
                    constructorParams[i] = _customContainer.AutofacContainer.Resolve(parameterType);
                else
                    constructorParams[i] = container.Resolve(parameterType);
            }

            return Activator.CreateInstance(instanceType, constructorParams);
        }
    }
}
