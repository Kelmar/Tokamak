using System;

namespace Tokamak.Core.Logging
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
