using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace ReadersTests.Support
{
    /// <summary>
    /// Serializes a minimal but spec-accurate binary FBX byte stream so the
    /// <c>BinaryFormatReader</c> can be exercised end to end.
    /// </summary>
    /// <remarks>
    /// Only the subset needed for the tests is implemented (scalars, strings and
    /// uncompressed numeric arrays).  Both the pre-7500 (32 bit lengths) and the
    /// 7500+ (64 bit lengths) node layouts are supported via <paramref name="version"/>.
    /// </remarks>
    internal sealed class FbxBinaryBuilder
    {
        // Matches FBXReader.BINARY_MAGIC (20 chars) plus the trailing null = 21 bytes.
        private const string MagicText = "Kaydara FBX Binary  ";

        public sealed class PropSpec
        {
            public byte[] Bytes { get; init; }
        }

        public sealed class NodeSpec
        {
            public string Name { get; init; }

            public List<PropSpec> Props { get; init; } = [];

            public List<NodeSpec> Children { get; init; } = [];
        }

        private readonly uint m_version;
        private readonly bool m_is64;

        public FbxBinaryBuilder(uint version = 7400)
        {
            m_version = version;
            m_is64 = version >= 7500;
        }

        public int SizeFieldLength => m_is64 ? 8 : 4;

        public int NullRecordLength => SizeFieldLength * 3 + 1;

        // --- Property factories ---------------------------------------------

        public static PropSpec Int(int value)
        {
            var b = new byte[5];
            b[0] = (byte)'I';
            BitConverter.GetBytes(value).CopyTo(b, 1);
            return new PropSpec { Bytes = b };
        }

        public static PropSpec Double(double value)
        {
            var b = new byte[9];
            b[0] = (byte)'D';
            BitConverter.GetBytes(value).CopyTo(b, 1);
            return new PropSpec { Bytes = b };
        }

        public static PropSpec Str(string value)
        {
            byte[] text = Encoding.UTF8.GetBytes(value);
            using var ms = new MemoryStream();
            ms.WriteByte((byte)'S');
            ms.Write(BitConverter.GetBytes((uint)text.Length));
            ms.Write(text);
            return new PropSpec { Bytes = ms.ToArray() };
        }

        /// <summary>Uncompressed int array property ('i').</summary>
        public static PropSpec IntArray(params int[] values)
        {
            using var ms = new MemoryStream();
            ms.WriteByte((byte)'i');
            ms.Write(BitConverter.GetBytes((uint)values.Length)); // element count
            ms.Write(BitConverter.GetBytes((uint)0));             // encoding 0 == uncompressed
            ms.Write(BitConverter.GetBytes((uint)(values.Length * sizeof(int)))); // byte length

            foreach (int v in values)
                ms.Write(BitConverter.GetBytes(v));

            return new PropSpec { Bytes = ms.ToArray() };
        }

        /// <summary>Uncompressed double array property ('d').</summary>
        public static PropSpec DoubleArray(params double[] values)
        {
            using var ms = new MemoryStream();
            ms.WriteByte((byte)'d');
            ms.Write(BitConverter.GetBytes((uint)values.Length));
            ms.Write(BitConverter.GetBytes((uint)0));
            ms.Write(BitConverter.GetBytes((uint)(values.Length * sizeof(double))));

            foreach (double v in values)
                ms.Write(BitConverter.GetBytes(v));

            return new PropSpec { Bytes = ms.ToArray() };
        }

        // --- Stream assembly -------------------------------------------------

        /// <summary>
        /// Builds the full file: 21 byte magic, 0x1A 0x00, version, the nodes and a
        /// terminating top level null record.
        /// </summary>
        public byte[] Build(params NodeSpec[] nodes)
        {
            using var ms = new MemoryStream();

            byte[] magic = Encoding.ASCII.GetBytes(MagicText); // 20 bytes
            ms.Write(magic);
            ms.WriteByte(0x00);   // 21st byte: null terminator
            ms.WriteByte(0x1A);
            ms.WriteByte(0x00);
            ms.Write(BitConverter.GetBytes(m_version));

            long offset = ms.Position;

            foreach (var node in nodes)
            {
                byte[] nb = Serialize(node, offset);
                ms.Write(nb);
                offset += nb.Length;
            }

            // Top level terminating null record.
            ms.Write(new byte[NullRecordLength]);

            return ms.ToArray();
        }

        private void WriteSize(BinaryWriter w, long value)
        {
            if (m_is64)
                w.Write((ulong)value);
            else
                w.Write((uint)value);
        }

        private byte[] Serialize(NodeSpec node, long start)
        {
            byte[] nameBytes = Encoding.UTF8.GetBytes(node.Name ?? string.Empty);
            byte[] propBytes = node.Props.SelectMany(p => p.Bytes).ToArray();

            int headerLen = SizeFieldLength * 3 + 1 + nameBytes.Length;
            long childStart = start + headerLen + propBytes.Length;

            // Serialize children at their running absolute offsets.
            using var childMs = new MemoryStream();
            long childOffset = childStart;

            foreach (var child in node.Children)
            {
                byte[] cb = Serialize(child, childOffset);
                childMs.Write(cb);
                childOffset += cb.Length;
            }

            if (node.Children.Count > 0)
            {
                // A node with children is terminated by a null record.
                childMs.Write(new byte[NullRecordLength]);
                childOffset += NullRecordLength;
            }

            byte[] childBytes = childMs.ToArray();
            long endOffset = childStart + childBytes.Length;

            using var ms = new MemoryStream();
            using var w = new BinaryWriter(ms);

            WriteSize(w, endOffset);
            WriteSize(w, node.Props.Count);
            WriteSize(w, propBytes.Length);

            w.Write((byte)nameBytes.Length);
            w.Write(nameBytes);
            w.Write(propBytes);
            w.Write(childBytes);
            w.Flush();

            return ms.ToArray();
        }
    }
}
