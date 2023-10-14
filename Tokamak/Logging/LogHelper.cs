using System;

namespace Tokamak.Logging
{
    public static class LogHelper
    {
        public static void Trace(this ILogger log, Exception ex, string format, params object[] args)
        {
            if (!log.LevelEnabled(LogLevel.Trace))
                return;

            log.Log(LogLevel.Trace, ex, format, args);
        }

        public static void Trace(this ILogger log, string format, params object[] args)
        {
            if (!log.LevelEnabled(LogLevel.Trace))
                return;

            log.Log(LogLevel.Trace, null, format, args);
        }

        public static void Debug(this ILogger log, Exception ex, string format, params object[] args)
        {
            if (!log.LevelEnabled(LogLevel.Debug))
                return;

            log.Log(LogLevel.Debug, ex, format, args);
        }

        public static void Debug(this ILogger log, string format, params object[] args)
        {
            if (!log.LevelEnabled(LogLevel.Debug))
                return;

            log.Log(LogLevel.Debug, null, format, args);
        }
        public static void Info(this ILogger log, Exception ex, string format, params object[] args)
        {
            if (!log.LevelEnabled(LogLevel.Info))
                return;

            log.Log(LogLevel.Info, ex, format, args);
        }

        public static void Info(this ILogger log, string format, params object[] args)
        {
            if (!log.LevelEnabled(LogLevel.Info))
                return;

            log.Log(LogLevel.Info, null, format, args);
        }

        public static void Warn(this ILogger log, Exception ex, string format, params object[] args)
        {
            if (!log.LevelEnabled(LogLevel.Warn))
                return;

            log.Log(LogLevel.Warn, ex, format, args);
        }

        public static void Warn(this ILogger log, string format, params object[] args)
        {
            if (!log.LevelEnabled(LogLevel.Warn))
                return;

            log.Log(LogLevel.Warn, null, format, args);
        }

        public static void Error(this ILogger log, Exception ex, string format, params object[] args)
        {
            if (!log.LevelEnabled(LogLevel.Error))
                return;

            log.Log(LogLevel.Error, ex, format, args);
        }

        public static void Error(this ILogger log, string format, params object[] args)
        {
            if (!log.LevelEnabled(LogLevel.Error))
                return;

            log.Log(LogLevel.Error, null, format, args);
        }

        public static void Fatal(this ILogger log, Exception ex, string format, params object[] args)
        {
            if (!log.LevelEnabled(LogLevel.Fatal))
                return;

            log.Log(LogLevel.Fatal, ex, format, args);
        }

        public static void Fatal(this ILogger log, string format, params object[] args)
        {
            if (!log.LevelEnabled(LogLevel.Fatal))
                return;

            log.Log(LogLevel.Fatal, null, format, args);
        }
    }
}
