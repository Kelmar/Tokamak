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
            .GetClientBuilder(args)
            .ConfigureServices(services =>
            {
                services.AddLogging<LogFactory>();
                //services.RegisterSingleton<IBackgroundService, GuiHost>();
            })
            .Build();
    }
}
