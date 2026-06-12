using System;

using Tokamak.Hosting.Abstractions;

using Tokamak.Hosting;
using Tokamak.Logging;

using Tokamak.Tritium;

using Tokamak.OGL;
using Tokamak.Assets;
using Tokamak.Silk.Input;

namespace TestBed
{
    class Program
    {
        static void Main(string[] args)
        {
            try
            {
                BuildHost(args)
                    .Run();
            }
            catch (Exception ex)
            {
                Console.Error.WriteLine("UNHANDLED ERROR: {0}", ex);
            }
        }

        static IGameHost BuildHost(string[] args) => GameHost
            .GetDefaultBuilder(args)
            .ConfigureAppConfiguration((env, config) =>
            {
                env.ApplicationName = "Tokamak Test Host";
            })
            .ConfigureServices(services =>
            {
                services.UseConsoleLogger();
                services.UseAssetManager();
                services.UseTritium();
                services.AllowOpenGL();
                services.UseSilkInput();

                services.RegisterScoped<PlayerController>();
            })
            .UseGameApp<TestGameApp>()
            .Build();
    }
}
