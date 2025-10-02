using Stashbox;

using Tokamak.Logging.Abstractions;

namespace Tokamak.Logging
{
    public static class LogExtensions
    {
        public static IStashboxContainer UseConsoleLogger(this IStashboxContainer container)
        {
            LogManager.SetFactory(new LogFactory());

            container.Register(typeof(ILogger<>), typeof(ConsoleLog<>));
            container.Register<ILogger, ConsoleLog>();

            return container;
        }
    }
}
