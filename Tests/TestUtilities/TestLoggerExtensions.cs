using Stashbox;
using Tokamak.Logging.Abstractions;

namespace TestUtilities;

public static class TestLoggerExtensions
{
    public static IStashboxContainer AddLogging(this IStashboxContainer container)
    {
        return container
            .Register<ILogger, TestLogger>()
            .Register(typeof(ILogger<>), typeof(TestLogger<>))
        ;
    }
}