using System;
using System.Collections.Generic;
using System.IO.Compression;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;

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
            byte[] data = m_reader.ReadExactly(length);
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

            object data = type switch
            {
                PropertyType.SignedShort => m_reader.ReadInt16(),
                PropertyType.Boolean => m_reader.ReadByte(),
                PropertyType.SignedInt => m_reader.ReadInt32(),
                PropertyType.Float => m_reader.ReadSingle(),
                PropertyType.Double => m_reader.ReadDouble(),
                PropertyType.SignedLong => m_reader.ReadInt64(),
                PropertyType.FloatArray => ReadFloatArray(),
                PropertyType.DoubleArray => ReadDoubleArray(),
                PropertyType.LongArray => ReadLongArray(),
                PropertyType.IntArray => ReadIntArray(),
                PropertyType.BoolArray => ReadBoolArray(),
                PropertyType.String => ReadPropertyString(),
                PropertyType.RawBinary => ReadPropertyRaw(),

                /*
                 * Unfortunately FBX doesn't tell us the size of a property,
                 * for a single value, so if we don't know the type, we
                 * can't recover. :(
                 */
                _ => throw new Exception($"Unknown property type '{c}'")
            };

            return new NodeProperty
            {
                Type = type,
                Data = data
            };
        }

        private static byte[] Decompress(byte[] data)
        {
            using var inStream = new MemoryStream(data);
            using var zipStream = new ZLibStream(inStream, CompressionMode.Decompress);
            using var outStream = new MemoryStream();

            zipStream.CopyTo(outStream);

            return outStream.ToArray();
        }

        private (byte[] data, int length) RawReadArray<T>()
        {
            int length = (int)m_reader.ReadUInt32();
            bool compressed = m_reader.ReadUInt32() == 1;
            int physicalLength = (int)m_reader.ReadUInt32();

            int itemSize = Marshal.SizeOf<T>();
            int recordSize = compressed ? physicalLength : (length * itemSize);

            byte[] data = m_reader.ReadExactly(recordSize);

            if (compressed)
            {
                data = Decompress(data);

                if ((data.Length / itemSize) != length ||
                    (data.Length % itemSize) != 0)
                {
                    throw new Exception("Data length error attempting to decompress data in FBX file.");
                }
            }

            return (data, length);
        }

        private byte[] ReadByteArray() => RawReadArray<byte>().data;

        private bool[] ReadBoolArray() => ReadByteArray().Select(b => b == 1).ToArray();

        private int[] ReadIntArray()
        {
            (byte[] data, int length) = RawReadArray<int>();
            int[] rval = new int[length];

            for (int i = 0; i < length; ++i)
                rval[i] = BitConverter.ToInt32(data, i * 4);

            return rval;
        }

        private long[] ReadLongArray()
        {
            (byte[] data, int length) = RawReadArray<long>();
            long[] rval = new long[length];

            for (int i = 0; i < length; ++i)
                rval[i] = BitConverter.ToInt64(data, i * 8);

            return rval;
        }

        private float[] ReadFloatArray()
        {
            (byte[] data, int length) = RawReadArray<float>();
            float[] rval = new float[length];

            for (int i = 0; i < length; ++i)
                rval[i] = BitConverter.ToSingle(data, i * 4);

            return rval;
        }

        private double[] ReadDoubleArray()
        {
            (byte[] data, int length) = RawReadArray<double>();
            double[] rval = new double[length];

            for (int i = 0; i < length; ++i)
                rval[i] = BitConverter.ToDouble(data, i * 8);

            return rval;
        }

        private string ReadPropertyString()
        {
            int length = (int)m_reader.ReadUInt32();
            return ReadString(length);
        }

        private byte[] ReadPropertyRaw()
        {
            int length = (int)m_reader.ReadUInt32();
            return m_reader.ReadExactly(length);
        }
    }
}
