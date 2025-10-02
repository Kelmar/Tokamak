using System;
using System.Reflection;

using Tokamak.Logging.Abstractions;

using Tokamak.Core.Utilities;

namespace Tokamak.Core.Logging
{
    /// <summary>
    /// Basic no frills logging to the console.
    /// </summary>
    public class ConsoleLog : ILogger
    {
        private readonly string m_name;

        public ConsoleLog(string name = "")
        {
            m_name = name;
        }

        public IDisposable BeginScope(object args)
        {
            return Indisposable.Instance;
        }

        public bool LevelEnabled(LogLevel level)
        {
            return true;
        }

        public void Log(LogLevel level, Exception? ex, string format, params object[] args)
        {
            string msg = args != null ? String.Format(format, args) : format;

            if (m_name != null)
                Console.WriteLine("{0} [{1}]: {2}", m_name, level, msg);
            else
                Console.WriteLine("{0}: {1}", level, msg);
        }
    }

    public class ConsoleLog<T> : ConsoleLog, ILogger<T>
    {
        public ConsoleLog()
            : base(GetLogName(typeof(T)))
        {
        }

        private static string GetLogName(Type t)
        {
            var attr = t.GetCustomAttribute<LogNameAttribute>();

            return attr?.Name ?? t.FullName ?? t.Name;
        }
    }
}
