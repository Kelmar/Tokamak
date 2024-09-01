using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Stashbox;

using Tokamak.Core.Utilities;

namespace Tokamak.Core.Drivers
{
    public static class Driver
    {
        public static IGameHostBuilder AddDrivers(this IGameHostBuilder builder, Action<IDriverRegistrar> register)
        {
            builder.ConfigureServices(services =>
            {
                services.TryRegister<IDriverRegistrar, DriverRegistrar>(cfg => cfg.WithSingletonLifetime());
                services.TryRegister<IDriverLoader, DriverLoader>(cfg => cfg.WithSingletonLifetime());

            });

            return builder;
        }
    }
}
