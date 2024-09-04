using Stashbox;

using Tokamak.Core;

using Tokamak.Tritium.Hosting;

namespace Tokamak.Tritium
{
    public static class Bootstrap
    {
        public static IStashboxContainer UseTritium(this IStashboxContainer container)
        {
            container.RegisterSingleton<IHostComponent, TritiumHostComponent>();

            return container;
        }
    }
}
