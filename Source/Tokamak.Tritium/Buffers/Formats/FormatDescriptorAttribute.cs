using System;

namespace Tokamak.Tritium.Buffers.Formats
{
    [AttributeUsage(AttributeTargets.Field)]
    internal class FormatDescriptorAttribute : Attribute
    {
        public FormatDescriptorAttribute(FormatBaseType baseType, int count)
        {
            BaseType = baseType;
            Count = count;
        }

        /// <summary>
        /// The primitive type that composes this field.
        /// </summary>
        public FormatBaseType BaseType { get; }

        /// <summary>
        /// The number of primitives in this field.
        /// </summary>
        public int Count { get; }
    }
}
