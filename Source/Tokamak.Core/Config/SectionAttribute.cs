using System;

namespace Tokamak.Core.Config
{
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Property, AllowMultiple = false, Inherited = false)]
    internal sealed class SectionAttribute : Attribute
    {
        public SectionAttribute(string name)
        {
            Section = name;
        }

        public string Section { get; }
    }
}
