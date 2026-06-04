using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Tokamak.Readers.FBX.DOM
{
    /// <summary>
    /// Node property of an FBX file.
    /// </summary>
    /// <remarks>
    /// Properties are not named in FBX files, but indexed.
    /// 
    /// This class contains some data conversion functions to
    /// help with translating the format into something useful.
    /// </remarks>
    internal abstract class NodeProperty
    {
        protected NodeProperty(PropertyType type)
        {
            Type = type;
        }

        public PropertyType Type { get; }

        public abstract bool AsBool();

        public abstract short AsShort();

        public abstract int AsInt();

        public abstract long AsLong();

        public abstract float AsFloat();

        public abstract double AsDouble();

        public abstract string AsString(Encoding? encoding = null);

        public override string ToString()
        {
            if (Type == PropertyType.RawBinary)
                return "[RAW BINARY]";

            return Type switch
            {
                // Scalars
                PropertyType.Boolean => AsBool().ToString(),
                PropertyType.SignedShort => AsShort().ToString(),
                PropertyType.SignedInt => AsInt().ToString(),
                PropertyType.SignedLong => AsLong().ToString(),
                PropertyType.Float => AsFloat().ToString(),
                PropertyType.Double => AsDouble().ToString(),

                // Arrays
                PropertyType.BoolArray => DumpArray<bool>(),
                PropertyType.IntArray => DumpArray<int>(),
                PropertyType.LongArray => DumpArray<long>(),
                PropertyType.FloatArray => DumpArray<float>(),
                PropertyType.DoubleArray => DumpArray<double>(),

                // Pseudo Arrays
                PropertyType.String => AsString(),
                PropertyType.RawBinary => "[RAW BINARY]",

                _ => "[UNKNOWN DATA TYPE]",
            };
        }

        public abstract T[] AsArrayOf<T>()
            where T : unmanaged;

        /// <summary>
        /// Used for dumping array data to a string.
        /// </summary>
        /// <remarks>
        /// This is really only helpful for debugging.
        /// </remarks>
        /// <typeparam name="T">The array data type.</typeparam>
        private string DumpArray<T>()
            where T : unmanaged
        {
            T[] items = AsArrayOf<T>();

            int count = items.Count();
            string itemStr = String.Join(", ", items.Take(Math.Min(count, 10)));

            return $"{typeof(T).Name}[{count}]: {itemStr}";
        }
    }

    /// <summary>
    /// Values are defined by the FBX format itself.
    /// </summary>
    public enum PropertyType : byte
    {
        SignedShort = (byte)'Y',
        Boolean     = (byte)'C',
        SignedInt   = (byte)'I',
        Float       = (byte)'F',
        Double      = (byte)'D',
        SignedLong  = (byte)'L',
        FloatArray  = (byte)'f',
        DoubleArray = (byte)'d',
        LongArray   = (byte)'l',
        IntArray    = (byte)'i',
        BoolArray   = (byte)'b',
        String      = (byte)'S',
        RawBinary   = (byte)'R',
    }

    public static class PropertyTypeEx
    {
        private static readonly HashSet<PropertyType> s_numericTypes =
        [
            PropertyType.Float,
            PropertyType.Double,
            PropertyType.SignedShort,
            PropertyType.SignedInt,
            PropertyType.SignedLong,
        ];

        private static readonly HashSet<PropertyType> s_arrayTypes =
        [
            PropertyType.FloatArray,
            PropertyType.DoubleArray,
            PropertyType.IntArray,
            PropertyType.BoolArray,
            PropertyType.LongArray
        ];

        extension (PropertyType type)
        {
            public bool IsNumeric => s_numericTypes.Contains(type);

            public bool IsArray => s_arrayTypes.Contains(type);
        }
    }
}
