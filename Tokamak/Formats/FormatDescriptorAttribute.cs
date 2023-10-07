using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tokamak.Formats
{
    [AttributeUsage(AttributeTargets.Field)]
    public sealed class FormatDescriptorAttribute : Attribute
    {
        public FormatDescriptorAttribute(FormatBaseType baseType, int count)
        {
            BaseType = baseType;
            Count = count;
        }

        public FormatBaseType BaseType { get; }

        public int Count { get; }
    }
}
