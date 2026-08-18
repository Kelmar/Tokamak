using System.Reflection;
using Tokamak.Logging.Abstractions;
using Tokamak.Utilities;

namespace TestUtilities;

public class TestLogger : ILogger
{
    private readonly string m_name;

    public TestLogger(string logName)
    {
        m_name = logName;
    }
    
    public void Log(LogLevel level, Exception? ex, string format, params object[]? args)
    {
        string msg = args is { Length: > 0 } ? string.Format(format, args) : format;
        
        if (ex != null)
            Console.WriteLine("{0}", ex);
        
        Console.WriteLine("{0} {1}: {2}", level, m_name, msg);
    }

    public bool LevelEnabled(LogLevel level) => true;

    public IDisposable BeginScope(object args) => Indisposable.Instance;
}

public class TestLogger<T> : TestLogger, ILogger<T>
{
    public TestLogger()
        : base(GetLogName(typeof(T)))
    {
    }
    
    private static string GetLogName(Type t)
    {
        var attr = t.GetCustomAttribute<LogNameAttribute>();

        return attr?.Name ?? t.FullName ?? t.Name;
    }
}