using Stashbox;

namespace Tokamak.Core.Logging
{
    public static class ConsoleLogFactory
    {
        public static IStashboxContainer UseConsoleLogger(this IStashboxContainer container)
        {
            container.Register(typeof(ILogger<>), typeof(ConsoleLog<>));
            container.Register<ILogger, ConsoleLog>();

            return container;
        }
    }
}
