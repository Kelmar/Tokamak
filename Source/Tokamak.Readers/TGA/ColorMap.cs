using System;
using System.IO;

namespace Tokamak.Readers.TGA
{
    internal class ColorMap
    {
        private readonly byte[] m_empty = [0, 0, 0, 0];
        private readonly BinaryReader m_reader;

        private byte[] m_data = [];

        public ColorMap(BinaryReader reader)
        {
            m_reader = reader;
        }

        public int Offset { get; set; }

        public int Length { get; set; }

        public int BitsPerPixel { get; set; }

        public int BytesPerPixel => Math.Max(1, BitsPerPixel / 8);

        /// <summary>
        /// Read TGA Color Map header data.
        /// </summary>
        /// <param name="reader"></param>
        public void ReadHeader()
        {
            Offset = m_reader.ReadUInt16(); // Start index of the entries, can be non-zero.
            Length = m_reader.ReadUInt16();
            BitsPerPixel = m_reader.ReadByte();
        }

        public void ReadData()
        {
            if (Length == 0)
                return; // No color map to load.

            m_data = m_reader.ReadExactly(Length * BytesPerPixel);
        }

        /// <summary>
        /// Gets indexed entry into the table.
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>
        public ReadOnlySpan<byte> GetEntry(int index)
        {
            index -= Offset;

            if (index < 0 || index > Length)
            {
                // Return black in case we're out of bounds.
                return m_empty.AsSpan(0, BytesPerPixel);
            }

            index *= BytesPerPixel;

            return m_data.AsSpan(index, BytesPerPixel);
        }
    }
}
