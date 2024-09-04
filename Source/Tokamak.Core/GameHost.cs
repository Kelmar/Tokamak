using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using Microsoft.Extensions.Configuration;

using Stashbox;

using Tokamak.Core.Hosting;

namespace Tokamak.Core
{
    public static class GameHost
    {
        public static IGameHostBuilder GetDefaultBuilder(string[] args = null)
        {
            var builder = new GameHostBuilder();
            builder.ConfigureDefaults(args);
            return builder;
        }

        public static IGameHostBuilder ConfigureDefaults(this IGameHostBuilder builder, string[] args)
        {
            //builder.UseGameApp<GameApp>();

            builder.ConfigureHostConfiguration(config => GetDefaultHostConfiguration(config, args));
            builder.ConfigureAppConfiguration(config => GetDefaultAppConfiguration(config, args));
            return builder;
        }

        private static void GetDefaultHostConfiguration(IConfigurationBuilder builder, string[] args)
        {
            var values = new Dictionary<string, string>();

            // TODO: Add environment variables
            // TODO: Add command line args here.
            
            if (values.Any())
                builder.AddInMemoryCollection(values);
        }

        private static void GetDefaultAppConfiguration(IConfigurationBuilder builder, string[] args)
        {
            builder
                .AddJsonFile("appsettings.json", optional: true, reloadOnChange: false)
                //.AddJsonFile($"appsettings.{env.EnvironmentName}.json", optional: true, reloadOnChange: false)
            ;

            var values = new Dictionary<string, string>();

            // TODO: Add environment variables
            // TODO: Add command line args here.

            if (values.Any())
                builder.AddInMemoryCollection(values);
        }

        public static IGameHostBuilder UseContainer<T>(this IGameHostBuilder builder, Func<IStashboxContainer> factory)
            where T : IStashboxContainer
        {
            builder.ContainerFactory = factory;
            return builder;
        }

        public static IGameHostBuilder UseContainer<T>(this IGameHostBuilder builder)
            where T : IStashboxContainer, new()
        {
            return builder.UseContainer<T>(() => new T());
        }

        public static IGameHostBuilder UseGameApp<T>(this IGameHostBuilder builder)
            where T : class, IGameApp
        {
            builder.ConfigureServices(c =>
            {
                if (c.IsRegistered<IGameApp>())
                    c.ReMap<IGameApp, T>(cfg => cfg.WithSingletonLifetime());
                else
                    c.RegisterSingleton<IGameApp, T>();
            });

            return builder;
        }

        public static void Run(this IGameHost host)
        {
            try
            {
                host.Start();
                host.MainLoop();
                host.Stop();
            }
            finally
            {
                host.Dispose();
            }
        }
    }
}
