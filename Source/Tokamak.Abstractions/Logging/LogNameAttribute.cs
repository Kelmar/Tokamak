using System;

namespace Tokamak.Abstractions.Logging
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
