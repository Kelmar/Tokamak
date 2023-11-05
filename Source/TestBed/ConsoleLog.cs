using System;

using Tokamak.Abstractions.Logging;

namespace TestBed
{
    public class ConsoleLog : ILogger
    {
        public IDisposable BeginScope(object args)
        {
            return null;
        }

        public bool LevelEnabled(LogLevel level)
        {
            return true;
        }

        public void Log(LogLevel level, Exception ex, string format, params object[] args)
        {
            string msg = args != null ? String.Format(format, args) : format;

            Console.WriteLine("{0}: {1}", level, msg);
        }
    }

    public class ConsoleLog<T> : ConsoleLog, ILogger<T>
    {

    }
}
