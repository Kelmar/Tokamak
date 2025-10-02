using System;

using Tokamak.Hosting.Abstractions;

using Tokamak.Hosting;
using Tokamak.Logging;

using Tokamak.Tritium;

using Tokamak.OGL;

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
            .ConfigureServices(services =>
            {
                services.UseConsoleLogger();
                services.UseTritium();
                services.AllowOpenGL();
            })
            .UseGameApp<TestGameApp>()
            .Build();
    }
}
