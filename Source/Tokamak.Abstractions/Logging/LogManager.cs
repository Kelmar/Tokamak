namespace Tokamak.Abstractions.Logging
{
    public static class LogManager
    {
        private static ILogFactory? s_factory = null;

        public static void SetFactory(ILogFactory factory) =>
            s_factory = factory;

        public static ILogger GetLogger(string name = "") =>
            s_factory?.Create(name) ?? new NullLogger(name);

        public static ILogger<T> GetLogger<T>() =>
            s_factory?.Create<T>() ?? new NullLogger<T>();
    }
}
