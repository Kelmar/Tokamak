using System;

using Silk.NET.Windowing;

using Tokamak.Core;
using Tokamak.Core.Logging;

namespace TestBed
{
    class Program
    {
        static void Main(string[] args)
        {
            try
            {
                BuildHost(args).Run();
            }
            catch (Exception ex)
            {
                Console.Error.WriteLine("UNHANDLED ERROR: {0}", ex);
            }
        }

        static IGameHost BuildHost(string[] args) => GameHost
            .GetBuilder(args)
            .ConfigureServices(services =>
            {
                services.AddLogging<LogFactory>();
                services.RegisterSingleton<IBackgroundService, GuiHost>();
            })
            .Build();
    }
}
