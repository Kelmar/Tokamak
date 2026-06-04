using System;
using System.Runtime.InteropServices;
using System.Text;

using Tokamak.Readers.FBX.DOM;

namespace Tokamak.Readers.FBX.Readers
{
    internal class BinaryNodeProperty : NodeProperty
    {
        private readonly ReadOnlyMemory<byte> m_data;

        public BinaryNodeProperty(PropertyType type, ReadOnlyMemory<byte> data)
            : base(type)
        {
            m_data = data;
        }

        public override bool AsBool()
        {
            // IEEE floating point numbers still store zero as zero.
            // Treat any non-zero value found as true.
            return m_data.Span.ContainsAnyExcept<byte>(0);
        }

        // Avoids some boxing/unboxing mechanisms, but still needs work for array bounds.

        public override short AsShort() => BitConverter.ToInt16(m_data.Span);

        public override int AsInt() => BitConverter.ToInt32(m_data.Span);

        public override long AsLong() => BitConverter.ToInt64(m_data.Span);

        public override float AsFloat() => BitConverter.ToSingle(m_data.Span);

        public override double AsDouble() => BitConverter.ToDouble(m_data.Span);

        public override string AsString(Encoding? encoding = null)
        {
            encoding ??= Encoding.UTF8; // Use ASCII by default?
            return encoding.GetString(m_data.Span);
        }

        public override T[] AsArrayOf<T>()
            => MemoryMarshal.Cast<byte, T>(m_data.Span).ToArray();
    }
}
