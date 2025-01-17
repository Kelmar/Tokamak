using System;
using System.Collections.Generic;
using System.IO;
using System.Numerics;

namespace Tokamak.Quill.Readers.TTF
{
    internal class ParseState
    {
        private static readonly DateTime TTF_EPOCH = new DateTime(1904, 1, 1, 0, 0, 0);

        private class PositionRestore : IDisposable
        {
            private readonly Stream m_stream;
            private readonly long m_save;

            public PositionRestore(Stream s, long newPosition)
            {
                m_stream = s;

                m_save = m_stream.Position;
                m_stream.Position = newPosition;
            }

            public void Dispose()
            {
                m_stream.Position = m_save;
            }
        }

        public ParseState(Stream input)
        {
            Input = input;
        }

        public Stream Input { get; }

        public FontFormat Format { get; set; }

        public Version Version { get; set; }

        public Version Revision { get; set; }

        public UInt32 ChecksumAdjust { get; set; }

        public UInt16 Flags { get; set; }

        /// <summary>
        /// Overall scaling factor for the font.
        /// </summary>
        /// <remarks>
        /// See the following for more details:
        /// https://en.wikipedia.org/wiki/Em_(typography)
        /// </remarks>
        public int UnitsPerEm { get; set; }

        public DateTime Created { get; set; }

        public DateTime Modified { get; set; }

        public Vector2 Min { get; set; }

        public Vector2 Max { get; set; }

        // Smallest readable size
        public int LowestRecPPEM { get; set; }

        public int FontDirectionHint { get; set; }

        public int LocationFormat { get; set; }

        public int GlyphCount { get; set; }

        public List<long> GlyphOffsets { get; set; } = new();

        public Dictionary<string, TableEntry> Tables { get; } = new();

        public Dictionary<NameId, string> Names { get; } = new();

        public List<ITTFGlyph> Glyphs { get; set; } = new();

        public ICharacterMapper CharMapper { get; set; }

        public string NameSearch(params NameId[] ids)
        {
            foreach (var id in ids)
            {
                if (Names.TryGetValue(id, out string rval))
                    return rval;
            }

            return String.Empty;
        }

        public IDisposable ReadContext(long newPosition)
        {
            return new PositionRestore(Input, newPosition);
        }

        public bool HasEntry(string name) => Tables.ContainsKey(name);

        public TableEntry JumpToEntry(string name)
        {
            if (!Tables.TryGetValue(name, out TableEntry rval))
                return null;

            JumpToEntry(rval);

            return rval;
        }

        public TableEntry JumpToEntryOrFail(string name)
        {
            TableEntry entry = JumpToEntry(name);

            if (entry == null)
                throw new FontFileException($"Unable to find {name} table");

            return entry;
        }

        public void JumpToEntry(TableEntry entry)
        {
            long pos = Input.Seek(entry.Offset, SeekOrigin.Begin);

            if (pos != entry.Offset)
                throw new FontFileException($"Unable to navigate to {entry.Tag} table");
        }

        private static void ByteSwap(byte[] b)
        {
            if (!BitConverter.IsLittleEndian)
                return; // TTF is already Big Endian

            if (b.Length == 4)
            {
                // 32-bit byte swap
                b[0] ^= b[3];
                b[3] ^= b[0];
                b[0] ^= b[3];

                b[2] ^= b[1];
                b[1] ^= b[2];
                b[2] ^= b[1];
            }
            else if (b.Length == 2)
            {
                // 16-bit byte swap
                b[0] ^= b[1];
                b[1] ^= b[0];
                b[0] ^= b[1];
            }
        }

        public byte SafeReadByte()
        {
            int b = Input.ReadByte();

            if (b == -1)
                throw new FormatException("Unexpected end of font file.");

            return (byte)b;
        }

        public byte[] ReadBytes(int len)
        {
            byte[] b = new byte[len];

            long res = Input.Read(b, 0, len);

            if (res != len)
                throw new Exception("Blah");

            return b;
        }

        public Int16 ReadInt16()
        {
            byte[] b = ReadBytes(2);
            ByteSwap(b);
            Int16 rval = BitConverter.ToInt16(b);
            return rval;
        }

        public UInt16 ReadUInt16()
        {
            byte[] b = ReadBytes(2);
            ByteSwap(b);
            return BitConverter.ToUInt16(b);
        }

        public Int32 ReadInt32()
        {
            byte[] b = ReadBytes(4);
            ByteSwap(b);
            return BitConverter.ToInt32(b);
        }

        public UInt32 ReadUInt32()
        {
            byte[] b = ReadBytes(4);
            ByteSwap(b);
            return BitConverter.ToUInt32(b);
        }

        public DateTime ReadLongDateTime()
        {
            byte[] b = ReadBytes(8);

            if (BitConverter.IsLittleEndian)
            {
                byte[] r = new byte[8];

                for (int i = 0; i < 8; ++i)
                    r[i] = b[7 - i];

                b = r;
            }

            long offset = BitConverter.ToInt64(b);

            return TTF_EPOCH.AddSeconds(offset);
        }

        public UInt16[] ReadUShorts(int len)
        {
            UInt16[] rval = new UInt16[len];

            for (int i = 0; i < len; ++i)
                rval[i] = ReadUInt16();

            return rval;
        }

        public Int16[] ReadShorts(int len)
        {
            Int16[] rval = new Int16[len];

            for (int i = 0; i < len; ++i)
                rval[i] = ReadInt16();

            return rval;
        }

        public UInt32[] ReadWords(int len)
        {
            UInt32[] rval = new uint[len];

            for (int i = 0; i < len; ++i)
                rval[i] = ReadUInt32();

            return rval;
        }

        /// <summary>
        /// Reads a TTF 2.14 fixed point number.
        /// </summary>
        public float ReadF2Dot14()
        {
            UInt16 val = ReadUInt16();

            // Whole part is upper two bits (two's comp, signed)

            bool signed = (val & 0x8000) != 0;

            // Sign extend to 32-bit integer
            byte[] b = [
                (byte)(signed ? 0xFF : 0),
                (byte)(signed ? 0xFF : 0),
                (byte)(signed ? 0xFF : 0),
                (byte)( ((val & 0x4000) != 0 ? 1 : 0) | (signed ? 0xFE : 0) )
            ];

            ByteSwap(b);

            int i = BitConverter.ToInt32(b);

            float fact = (val & 0x3FFF) / 16384.0f;

            return i + fact;
        }
    }
}
