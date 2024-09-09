namespace Tokamak.Core.Logging
{
    public static class LogManager
    {
        public static ILogger GetLogger(string name = null) => new ConsoleLog(name);

        public static ILogger<T> GetLogger<T>() => new ConsoleLog<T>();
    }
}
