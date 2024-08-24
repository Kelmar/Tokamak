using System;
using System.Threading.Tasks;

using Tokamak.Core.Services;

namespace Tokamak.Core
{
    public static class GameHost
    {
        public static GameHostBuilder GetBuilder(string[] args = null)
        {
            var rval = new GameHostBuilder();
            rval.ConfigureArguments(args);
            return rval;
        }

        public static GameHostBuilder UseServiceLocator<T>(this GameHostBuilder builder, Func<IServiceLocator> factory)
            where T : IServiceLocator
        {
            builder.ServiceLocatorFactory = factory;
            return builder;
        }

        public static GameHostBuilder UseServiceLocator<T>(this GameHostBuilder builder)
            where T : IServiceLocator, new()
        {
            return builder.UseServiceLocator<T>(() => new T());
        }

        public static void Run(this IGameHost host)
        {
            try
            {
                host.StartAsync().GetAwaiter().GetResult();
                host.StopAsync().GetAwaiter().GetResult();
            }
            finally
            {
                host.Dispose();
            }
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
                host.Dispose();
            }
        }
    }
}
