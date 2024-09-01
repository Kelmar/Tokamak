using System;
using System.Threading.Tasks;

using Tokamak.Core;
using Tokamak.Core.Logging;

namespace TestBed
{
    class Program
    {
        static async Task Main(string[] args)
        {
            try
            {
                await BuildHost(args).RunAsync();
            }
            catch (Exception ex)
            {
                Console.Error.WriteLine("UNHANDLED ERROR: {0}", ex);
            }
        }

        static IGameHost BuildHost(string[] args) => GameHost
            .GetDefaultClientBuilder(args)
            .ConfigureServices(services =>
            {
                services.UseConsoleLogger();
                //services.RegisterSingleton<IBackgroundService, GuiHost>();
            })
            .Build();
    }
}
