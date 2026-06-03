using System;
using System.Collections.Generic;
using System.Linq;

namespace Tokamak.Readers.FBX
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
    internal class NodeProperty
    {
        public required PropertyType Type { get; internal set; }

        public required object Data { get; internal set; }

        public bool AsBool() => Convert.ToBoolean(Data);

        public int AsInt() => Convert.ToInt32(Data);

        public long AsLong() => Convert.ToInt64(Data);

        public string AsString() => Convert.ToString(Data) ?? String.Empty;

        public override string ToString()
        {
            if (Type == PropertyType.RawBinary)
                return "[RAW BINARY]";

            return Type switch
            {
                PropertyType.FloatArray => DumpArray<float>(),
                PropertyType.DoubleArray => DumpArray<double>(),
                PropertyType.LongArray => DumpArray<long>(),
                PropertyType.IntArray => DumpArray<int>(),
                PropertyType.BoolArray => DumpArray<bool>(),
                _ => Data.ToString() ?? "[NULL]",
            };
        }

        public IEnumerable<T> AsEnumerable<T>()
            where T : unmanaged
        {
            return Type switch
            {
                PropertyType.FloatArray => ConvertTo<float, T>(),
                PropertyType.DoubleArray => ConvertTo<double, T>(),
                PropertyType.LongArray => ConvertTo<long, T>(),
                PropertyType.IntArray => ConvertTo<int, T>(),
                PropertyType.BoolArray => ConvertTo<bool, T>(),
                _ => throw new Exception($"Not an array type {Type}")
            };
        }

        private IEnumerable<TTo> ConvertTo<TFrom, TTo>()
            where TFrom : unmanaged
            where TTo : unmanaged
        {
            Type t = typeof(TTo);

            var rdr = (IEnumerable<TFrom>?)Data;
            return rdr?.Select(i => (TTo)Convert.ChangeType(i, t)) ?? [];
        }

        /// <summary>
        /// Used for dumping array data to a string.
        /// </summary>
        /// <remarks>
        /// This is really only helpful for debugging.
        /// </remarks>
        /// <typeparam name="T">The array data type.</typeparam>
        private string DumpArray<T>()
            where T : struct
        {
            IEnumerable<T> items = (Data as IEnumerable<T>) ?? [];

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
