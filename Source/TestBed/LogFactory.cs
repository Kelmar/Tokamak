using Tokamak.Abstractions.Logging;

namespace TestBed
{
    public class LogFactory : ILogFactory
    {
        public ILogger GetLogger(string name)
        {
            return new ConsoleLog();
        }

        public ILogger<T> GetLogger<T>()
        {
            return new ConsoleLog<T>();
        }
    }
}
