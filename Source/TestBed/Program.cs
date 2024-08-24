using System;

using Silk.NET.Windowing;

using Tokamak.Core;

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
                services.Register<IBackgroundService>(new GuiHost());
            })
            .Build();
    }
}
