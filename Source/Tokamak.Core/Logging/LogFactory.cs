using Tokamak.Abstractions.Logging;

namespace Tokamak.Core.Logging
{
    internal class LogFactory : ILogFactory
    {
        public ILogger Create(string name = "") => new ConsoleLog(name);

        public ILogger<T> Create<T>() => new ConsoleLog<T>();
    }
}
