using System;

namespace Tokamak.Logging.Abstractions
{
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = false)]
    public sealed class LogNameAttribute : Attribute
    {
        public LogNameAttribute(string name)
        {
            Name = name;
        }

        public string Name { get; }
    }
}
