using System;
using System.Reflection;

namespace Tokamak.Abstractions.Logging
{
    /// <summary>
    /// Void logger
    /// </summary>
    internal class NullLogger : ILogger
    {
        private class Indisposable : IDisposable { public void Dispose() { } }

        private static Indisposable s_indisposable = new Indisposable();

        public NullLogger(string name = "")
        {
            LogName = name;
        }

        public string LogName { get; }

        public IDisposable BeginScope(object args) => s_indisposable;

        public bool LevelEnabled(LogLevel level) => false;

        public void Log(LogLevel level, Exception? ex, string format, params object[] args)
        {
        }
    }

    internal class NullLogger<T> : NullLogger, ILogger<T>
    {
        public NullLogger()
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
