using Tokamak.Core.Config;
using Tokamak.Core.Hosting;

namespace Tokamak.Core
{
    public static class GameHost
    {
        public static IGameHost Instance { get; internal set; }

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

        private static void GetDefaultHostConfiguration(IConfigBuilder builder, string[] args)
        {
            builder.AddEnvironmentVariables("tokamak", "^TOKAMAK_(.*)");

            // TODO: Add command line args here.
        }

        private static void GetDefaultAppConfiguration(IConfigBuilder builder, string[] args)
        {
            builder
                .AddJsonFile("appsettings.json", optional: true)
                //.AddJsonFile($"appsettings.{env.EnvironmentName}.json", optional: true)
            ;

            // TODO: Add environment variables
            // TODO: Add command line args here.
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
