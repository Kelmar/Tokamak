using System;
using System.Collections.Generic;
using System.IO.Compression;
using System.IO;
using System.Text;

using Tokamak.Readers.FBX.DOM;

namespace Tokamak.Readers.FBX.Readers
{
    internal class BinaryFormatReader : IParser
    {
        private readonly Stream m_input;
        private readonly BinaryReader m_reader;
        private readonly Encoding m_encoding;

        private readonly Func<UInt64> m_readSize;

        public BinaryFormatReader(Stream input, Encoding encoding)
        {
            m_input = input;
            m_reader = new BinaryReader(m_input, encoding, true);
            m_encoding = encoding;

            // Magic has already been read.
            m_input.Seek(21, SeekOrigin.Begin);

            m_input.Seek(2, SeekOrigin.Current); // Ignore 0x1A 0x00 (validate these bytes?)

            /*
             * A document suggests that byte 22 might be an endian flag:
             * 0x00 = Little endian
             * 0x01 = Big endian
             */
            
            uint version = m_reader.ReadUInt32();

            m_readSize = version < 7500 ?
                () => m_reader.ReadUInt32() :
                () => m_reader.ReadUInt64(); // Version 7.5 and later uses 64-bit lengths for nodes.
        }

        private string ReadString(int length)
        {
            byte[] data = new byte[length];
            m_reader.ReadExactly(data);

            string s = m_encoding.GetString(data);

            /*
             * The format and .NET are quite happy to accept garbage in this string.
             * I suspect these are just being written as flat buffers by Blender.
             * Trim staring at null character.
             */

            /*
             * Turns out this is not garbage, like I thought it might have been.
             * 
             * Apparently the FBX standard uses this to denote some sort of class/sub-class
             * pair.  In the text file version this would be denoted with "::", but in the binary
             * file for some reason this is denoted with 0x00, 0x01....
             * 
             * For now we'll just replace this sequence with "::" to match that of the text
             * file version and to make parsing easier elsewhere when we start sorting
             * through the read in nodes.
             */

            s = s.Replace("\0\x01", "::");

            // If we still have what looks like a null character, we'll continue to trim.

            int idx = s.IndexOf('\0');

            if (idx > -1)
                s = s.Substring(0, idx);

            return s;
        }

        public Node? ReadNode()
        {
            long startPos = m_input.Position;

            ulong endOffset = m_readSize();   // Expected stream position after read
            ulong numProps = m_readSize();    // Count of properties
            ulong propListLen = m_readSize(); // Length of properties in bytes

            byte nameLen = m_reader.ReadByte();

            if (endOffset == 0 && nameLen == 0)
                return null;

            string name = ReadString(nameLen);

            var properties = new List<NodeProperty>((int)numProps);
            var children = new List<Node>();

            for (ulong i = 0; i < numProps; ++i)
                properties.Add(ReadProperty());

            // Start reading nested nodes until "null"
            while ((ulong)m_input.Position < endOffset)
            {
                var nested = ReadNode();

                if (nested == null)
                    break;

                children.Add(nested);
            }

            var result = new Node
            {
                Name = name,
                Children = children,
                Properties = properties
            };

            /*
             * While this might be handy for some sanity checks, we area effectively relying
             * on whatever that wrote the FBX file got it right to begin with.  Or that it
             * didn't get corrupted on disk somehow.
             */
            //Debug.Assert(
            //    endOffset == (ulong)m_input.Position,
            //    "FBX read desync issue, stream position is not the expected value");

            return result;
        }

        private NodeProperty ReadProperty()
        {
            char c = (char)m_reader.ReadByte();
            PropertyType type = (PropertyType)c;

            byte[] data = type switch
            {
                // Scalars
                PropertyType.Boolean => [m_reader.ReadByte()],
                PropertyType.SignedShort => ReadScalar(sizeof(Int16)),
                PropertyType.SignedInt => ReadScalar(sizeof(Int32)),
                PropertyType.SignedLong => ReadScalar(sizeof(Int64)),
                PropertyType.Float => ReadScalar(sizeof(Single)),
                PropertyType.Double => ReadScalar(sizeof(Double)),

                // Arrays
                PropertyType.BoolArray => ReadArrayData(1),
                PropertyType.IntArray => ReadArrayData(sizeof(Int32)),
                PropertyType.LongArray => ReadArrayData(sizeof(Int64)),
                PropertyType.FloatArray => ReadArrayData(sizeof(Single)),
                PropertyType.DoubleArray => ReadArrayData(sizeof(Double)),

                // Array like....
                PropertyType.String => ReadPropertyRaw(),
                PropertyType.RawBinary => ReadPropertyRaw(),

                /*
                 * Unfortunately FBX doesn't tell us the size of a property,
                 * for a single value, so if we don't know the type, we
                 * can't recover. :(
                 */
                _ => throw new Exception($"Unknown property type '{c}'")
            };

            return new BinaryNodeProperty(type, data);
        }

        private byte[] ReadScalar(int itemSize)
        {
            byte[] data = new byte[itemSize];
            m_reader.ReadExactly(data);
            return data;
        }

        private static byte[] Decompress(byte[] data)
        {
            using var inStream = new MemoryStream(data);
            using var zipStream = new ZLibStream(inStream, CompressionMode.Decompress);
            using var outStream = new MemoryStream();

            zipStream.CopyTo(outStream);

            return outStream.ToArray();
        }

        private byte[] ReadArrayData(int itemSize)
        {
            int length = (int)m_reader.ReadUInt32();
            bool compressed = m_reader.ReadUInt32() == 1;
            int physicalLength = (int)m_reader.ReadUInt32();

            int recordSize = compressed ? physicalLength : (length * itemSize);

            byte[] data = new byte[recordSize];
            m_reader.ReadExactly(data);

            if (compressed)
            {
                data = Decompress(data);

                if ((data.Length / itemSize) != length ||
                    (data.Length % itemSize) != 0)
                {
                    throw new Exception("Data length error attempting to decompress data in FBX file.");
                }
            }

            return data;
        }

        private byte[] ReadPropertyRaw()
        {
            int length = (int)m_reader.ReadUInt32();
            byte[] data = new byte[length];
            m_reader.ReadExactly(data);
            return data;
        }
    }
}
