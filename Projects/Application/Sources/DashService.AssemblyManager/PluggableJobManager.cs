using System;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using Autofac;
using DashService.Context;
using DashService.Context.Models;
using DashService.Framework;
using DashService.Job.Abstraction;

namespace DashService.JobHandler
{
    public static class PluggableJobManager
    {
        [MethodImpl(MethodImplOptions.NoInlining)]
        public static PluginableJobModel Load(string jobsPath)
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
            MethodInfo methodInfo = jobType.GetMethod("Register");
            if (methodInfo == null)
                throw new Exception();

            methodInfo.Invoke(null, new object[] { containerBuilder });
            var container = containerBuilder.Build();
            var jobInstance = (IJob)Instance(jobType, container);

            var pluginableJobModel = new PluginableJobModel(jobInstance, assembly, alc, alcWeakRef);

            JobContainer.Register(pluginableJobModel);

            return pluginableJobModel;
        }

        [MethodImpl(MethodImplOptions.NoInlining)]
        public static void LoadDirectory(string pluginsPath)
        {
            foreach (var directory in Directory.GetDirectories(pluginsPath))
                PluggableJobManager.Load(directory);
        }

        [MethodImpl(MethodImplOptions.NoInlining)]
        public static void Unload(IPluginableJobModel pluginableJobModel)
        {
            pluginableJobModel.HostAssemblyLoadContext.Unload();
        }

        [MethodImpl(MethodImplOptions.NoInlining)]
        private static object Instance(Type jobType, IContainer container)
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
                if (Context.Autofac.Container.IsRegistered(parameterType))
                    constructorParams[i] = Context.Autofac.Container.Resolve(parameterType);
                else
                    constructorParams[i] = container.Resolve(parameterType);
            }

            return Activator.CreateInstance(instanceType, constructorParams);
        }
    }
}
