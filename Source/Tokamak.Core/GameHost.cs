using Tokamak.Core.Config;
using Tokamak.Core.Hosting;

namespace Tokamak.Core
{
    public static class GameHost
    {
        /// <summary>
        /// Gets a builder that constructs the default generic GameHost.
        /// </summary>
        /// <param name="args">Application command line arguments.</param>
        public static IGameHostBuilder GetDefaultBuilder(string[]? args = null)
        {
            var builder = new GameHostBuilder();
            builder.ConfigureDefaults(args ?? []);
            return builder;
        }

        public static IGameHostBuilder ConfigureDefaults(this IGameHostBuilder builder, string[] args)
        {
            //builder.UseGameApp<GameApp>();

            builder.ConfigureHostConfiguration(config => GetDefaultHostConfiguration(config, args));
            builder.ConfigureAppConfiguration((env, config) => GetDefaultAppConfiguration(env, config, args));
            return builder;
        }

        private static void GetDefaultHostConfiguration(IConfigBuilder builder, string[] args)
        {
            builder.AddEnvironmentVariables("tokamak", "^TOKAMAK_(.*)");
            builder.ParseCommandArgs(args);
        }

        private static void GetDefaultAppConfiguration(IHostEnvironment env, IConfigBuilder builder, string[] args)
        {
            builder
                .AddJsonFile("appsettings.json", optional: true)
                .AddJsonFile($"appsettings.{env.EnvironmentName}.json", optional: true)
            ;

            builder.AddEnvironmentVariables("tokamak", "^TOKAMAK_(.*)");
            builder.ParseCommandArgs(args);
        }

        /// <summary>
        /// Method for selecting which GameApp implementation to use.
        /// </summary>
        /// <remarks>
        /// If an implementation isn't selected, then a default generic GameApp will be used.
        /// </remarks>
        /// <typeparam name="T">The type of GameApp to use.</typeparam>
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

        /// <summary>
        /// Utility method for starting, running, stopping and 
        /// disposing of the built up GameHost object.
        /// </summary>
        /// <param name="host"></param>
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
