using System;

namespace Tokamak.Core.Logging
{
    public interface ILogger
    {
        public void Log(LogLevel level, Exception ex, string format, params object[] args);

        public bool LevelEnabled(LogLevel level);

        public IDisposable BeginScope(object args);
    }

    public interface ILogger<in T> : ILogger
    {
    }
}
