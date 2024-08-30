using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using Microsoft.Extensions.Configuration;

using Stashbox;

using Tokamak.Core.Implementation;

namespace Tokamak.Core
{
    public static class GameHost
    {
        public static IGameHostBuilder GetClientBuilder(string[] args = null)
        {
            var rval = new ClientHostBuilder();
            rval.ConfigureDefaults(args);
            return rval;
        }

        public static IGameHostBuilder ConfigureDefaults(this IGameHostBuilder builder, string[] args)
        {
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
                builder.AddInMemoryCollection(values.ToArray());
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
                builder.AddInMemoryCollection(values.ToArray());
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

        public static void Run(this IGameHost host)
        {
            host.RunAsync().GetAwaiter().GetResult();
        }

        public static async Task RunAsync(this IGameHost host)
        {
            try
            {
                await host.StartAsync();
                await host.StopAsync();
            }
            finally
            {
                if (host is IAsyncDisposable asyncDisposable)
                    await asyncDisposable.DisposeAsync();
                else
                    host.Dispose();
            }
        }
    }
}
