using Stashbox;

using Tokamak.Hosting.Abstractions;

using Tokamak.Tritium.APIs;
using Tokamak.Tritium.APIs.NullRender;
using Tokamak.Tritium.Hosting;

namespace Tokamak.Tritium
{
    public static class Bootstrap
    {
        public static IStashboxContainer UseTritium(this IStashboxContainer container)
        {
            container.RegisterSingleton<IHostComponent, TritiumHostComponent>();

            container.Register<GraphicsLoader>();
            container.RegisterSingleton<NullAPI>();

            container.Register<IGraphicsLayer>(options => options
                .WithSingletonLifetime()
                .WithFactory<GraphicsLoader>(loader => loader.Build())
            );

            return container;
        }
    }
}
