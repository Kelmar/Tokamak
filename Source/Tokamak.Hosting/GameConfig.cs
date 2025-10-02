using System;

using Stashbox;

using Tokamak.Utilities;

using Tokamak.Config.Abstractions;

using Tokamak.Hosting.Abstractions;
using Tokamak.Hosting.Config;

namespace Tokamak.Hosting
{
    public static class GameConfig
    {
        public static IGameHostBuilder Configure<TOptions>(this IGameHostBuilder builder, TOptions options)
            where TOptions : class
        {
            builder.ConfigureServices(cfg => cfg.RegisterInstance(new StaticOptions<TOptions>(options)));

            return builder;
        }

        public static IGameHostBuilder Configure<TOptions>(this IGameHostBuilder builder, Action<IConfigOptions<TOptions>>? configurator = null)
            where TOptions : class
        {
            builder.ConfigureServices(cfg =>
            {
                cfg.TryRegister<IConfigOptions<TOptions>>(new ConfigOptions<TOptions>());

                if (configurator != null)
                {
                    var options = cfg.Resolve<IConfigOptions<TOptions>>();

                    configurator(options);
                }
            });

            return builder;
        }

        public static IConfigOptions<TOptions> UseSection<TOptions>(this IConfigOptions<TOptions> options, string name)
            where TOptions : class
        {
            options.Section = name;
            return options;
        }
    }
}
