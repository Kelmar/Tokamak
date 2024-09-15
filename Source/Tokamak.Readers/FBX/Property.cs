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
    internal class Property
    {
        public PropertyType Type { get; internal set; }

        public object Data { get; internal set; }

        public int AsInt() => Convert.ToInt32(Data);

        public string AsString() => Convert.ToString(Data);

        public override string ToString()
        {
            if (Type == PropertyType.RawBinary)
                return "[RAW BINARY]";

            switch (Type)
            {
            case PropertyType.FloatArray:
                return DumpArray<float>();

            case PropertyType.DoubleArray:
                return DumpArray<double>();

            case PropertyType.LongArray:
                return DumpArray<long>();

            case PropertyType.IntArray:
                return DumpArray<int>();

            case PropertyType.BoolArray:
                return DumpArray<bool>();
            }

            return Data?.ToString() ?? "[NULL]";
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

            var rdr = (IEnumerable<TFrom>)Data;
            return rdr.Select(i => (TTo)Convert.ChangeType(i, t));
        }

        private IEnumerable<T> GetReader<T>()
            where T : unmanaged
        {
            return (IEnumerable<T>)Data;
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
            IEnumerable<T> items = (Data as IEnumerable<T>) ?? new List<T>();

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
}
