using System;
using System.Threading.Tasks;

using Stashbox;

using Tokamak.Core.Implementation;

namespace Tokamak.Core
{
    public static class GameHost
    {
        public static IGameHostBuilder GetClientBuilder(string[] args = null)
        {
            var rval = new ClientHostBuilder();
            rval.ConfigureArguments(args);
            return rval;
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
