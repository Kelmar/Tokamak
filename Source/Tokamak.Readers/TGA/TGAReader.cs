using System;
using System.Diagnostics;
using System.IO;

using Tokamak.Tritium.Buffers;
using Tokamak.Tritium.Buffers.Formats;

namespace Tokamak.Readers.TGA
{
    /// <summary>
    /// Reader for Truevision TGA "TARGA" format.
    /// </summary>
    /// <remarks>
    /// Lots of room for improvement here.
    /// </remarks>
    public sealed class TGAReader : IDisposable
    {
        public enum FormatType
        {
            /// <summary>
            /// File contains no image data.
            /// </summary>
            None = 0,

            UncompressedColorMapped = 1,
            UncompressedTrueColor = 2,
            UncompressedGrayscale = 3,

            RunLengthEncodedColorMap = 9,
            RunLengthEncodedTrueColor = 10,
            RunLengthEncodedGrayscale = 11
        }

        private readonly bool m_closeStream;
        private readonly Stream m_input;
        private readonly BinaryReader m_reader;

        private readonly ColorMap m_colorMap;

        private bool m_useColorMap = false;
        private bool m_useRLE = false;

        private int m_width = 0;
        private int m_height = 0;
        private int m_bitsPerPixel = 0;

        private Bitmap? m_result;

        private int m_bytesPerPixel;
        private int m_offset;
        private int m_linePitchRemainder;

        private int m_x;
        private int m_y;

        private PixelFormat m_pixelFormat = PixelFormat.FormatA8;

        public TGAReader(Stream input, bool closeStream = true)
        {
            ArgumentNullException.ThrowIfNull(input, nameof(input));

            if (!input.CanRead)
                throw new ArgumentException("Stream not open for reading", nameof(input));

            m_closeStream = closeStream;
            m_input = input;
            m_reader = new BinaryReader(input);
            m_colorMap = new ColorMap(m_reader);
        }

        public TGAReader(string filename)
            : this(File.OpenRead(filename))
        {
        }

        public void Dispose()
        {
            if (m_closeStream)
                m_input.Dispose();

            GC.SuppressFinalize(this);
        }

        public FormatType Format { get; private set; } = FormatType.None;

        private void ValidateFormatType(FormatType format)
        {
            m_useRLE = ((int)format & 0x08) != 0;

            switch (format)
            {
            case FormatType.None:
                throw new FileFormatException("TGA file contains no image data.");

            case FormatType.UncompressedColorMapped:
                m_useColorMap = true;
                break;

            case FormatType.UncompressedTrueColor:
            case FormatType.UncompressedGrayscale:
                break;

            case FormatType.RunLengthEncodedColorMap:
                m_useColorMap = true;
                break;

            case FormatType.RunLengthEncodedTrueColor:
            case FormatType.RunLengthEncodedGrayscale:
                break;

            default:
                throw new FileFormatException($"Unknown TGA image format: {(int)format}");
            }
        }

        private void ReadHeader()
        {
            // Image ID length (Field 1)
            byte idLength = m_reader.ReadByte();

            // Color map type (Field 2)
            m_input.Seek(1, SeekOrigin.Current); // Skip color type
            //byte colorMapType = m_reader.ReadByte();

            // Image type (Field 3)
            Format = (FormatType)m_reader.ReadByte();

            ValidateFormatType(Format);

            // Color map specification (Field 4)  - 5 bytes
            m_colorMap.ReadHeader();

            // Image Specification (Field 5)  - 10 bytes

            m_input.Seek(4, SeekOrigin.Current); // Skip x & y origin values.
            //int xOrigin = m_reader.ReadUInt16(); // 2 bytes
            //int yOrigin = m_reader.ReadUInt16(); // 2 bytes

            m_width = m_reader.ReadUInt16(); // 2 bytes
            m_height = m_reader.ReadUInt16(); // 2 bytes

            // Should be 8, 15, 16 (15 + 1-bit alpha), 24, or 32
            m_bitsPerPixel = m_reader.ReadByte(); // 1 byte

            m_pixelFormat = m_bitsPerPixel switch
            {
                8 => PixelFormat.FormatA8,
                15 => PixelFormat.FormatR5G5B5A1,
                16 => PixelFormat.FormatR5G5B5A1,
                24 => PixelFormat.FormatR8G8B8,
                32 => PixelFormat.FormatR8G8B8A8,
                _ => throw new FileFormatException($"Unknown bit depth in TGA file: {m_bitsPerPixel}")
            };

            m_input.Seek(1, SeekOrigin.Current); // Skip image descriptor.
            //byte imageDescriptor = m_reader.ReadByte(); // 1 byte

            // Image ID (Field 6)
            m_input.Seek(idLength, SeekOrigin.Current); // Skip the ID field, we don't really care about it.
        }

        /// <summary>
        /// Read a single pixel of color data.
        /// </summary>
        /// <remarks>
        /// Data is automatically swizzled before return.
        /// </remarks>
        private byte[] ReadSinglePixel()
        {
            byte[] data;

            if (m_useColorMap)
            {
                byte index = m_reader.ReadByte();

                ReadOnlySpan<byte> entry = m_colorMap.GetEntry(index);

                data = entry.ToArray();
            }
            else
            {
                data = m_reader.ReadExactly(m_bytesPerPixel);
            }

            SwizzleBits(data);

            return data;
        }

        private void SwizzleBits(byte[] data)
        {
            Debug.Assert(m_result != null);

            switch (m_result.Format)
            {
            case PixelFormat.FormatR5G6B5:
                // Swap bytes
                (data[0], data[1]) = (data[1], data[0]);
                break;

            case PixelFormat.FormatR5G5B5A1:
                // Swap bytes
                (data[0], data[1]) = (data[1], data[0]);

                // TGA format is A1R5G5B5
                // Need R5G5B5A1
                bool transparent = (data[0] & 0x80) == 0;

                // Shift bits by 1
                data[0] <<= 1;
                data[0] |= (byte)((data[1] & 0x80) != 0 ? 1 : 0);
                data[1] <<= 1;
                data[1] |= (byte)(transparent ? 0 : 1);
                break;

            case PixelFormat.FormatR8G8B8A8:
                data[3] = (byte)(255 - data[3]); // Invert alpha
                goto case PixelFormat.FormatR8G8B8; // Also process same as R8G8B8

            case PixelFormat.FormatR8G8B8:
                (data[0], data[2]) = (data[2], data[0]); // Swap blue and red order
                break;
            }
        }

        private void AddSinglePixel(byte[] data)
        {
            Debug.Assert(m_result != null);

            Buffer.BlockCopy(data, 0, m_result.Data, m_offset, m_bytesPerPixel);

            m_offset += m_bytesPerPixel;

            ++m_x;

            if (m_x >= m_result.Size.X)
            {
                m_offset += m_linePitchRemainder;

                m_x = 0;

                if (++m_y > m_result.Size.Y)
                    throw new FileFormatException("Invalid TGA file format.");
            }
            else if (m_y == m_result.Size.Y)
                throw new FileFormatException("Invalid TGA file format.");

            Debug.Assert(m_offset <= m_result.Size.Y * m_result.Pitch);
        }

        private void DoRLLPacket()
        {
            byte header = m_reader.ReadByte();
            bool isRun = (header & 0x80) != 0;
            int count = (header & 0x7F) + 1;

            if (isRun)
            {
                byte[] data = ReadSinglePixel();

                for (; count > 0; --count)
                    AddSinglePixel(data);
            }
            else
            {
                for (; count > 0; --count)
                {
                    byte[] data = ReadSinglePixel();

                    SwizzleBits(data);
                    AddSinglePixel(data);
                }
            }
        }

        private void DoPixel()
        {
            byte[] data = ReadSinglePixel();
            AddSinglePixel(data);
        }

        public Bitmap Import()
        {
            ReadHeader();

            m_colorMap.ReadData();

            m_result = new Bitmap(m_width, m_height, m_pixelFormat);

            m_bytesPerPixel = Math.Max(1, m_bitsPerPixel / 8);
            m_linePitchRemainder = m_result.Pitch - m_result.WidthBytes;
            m_offset = 0;

            while ((m_x < m_result.Size.X) && (m_y < m_result.Size.Y))
            {
                if (m_useRLE)
                    DoRLLPacket();
                else
                    DoPixel();
            }

            m_result.Invalidate();

            return m_result;
        }
    }
}
