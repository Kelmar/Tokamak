using System;
using System.Collections.Generic;
using System.IO.Compression;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;

namespace Tokamak.Readers.FBX
{
    internal class BinaryFormatReader : IParser
    {
        private readonly Stream m_input;
        private readonly BinaryReader m_reader;
        private readonly Encoding m_encoding;

        public BinaryFormatReader(Stream input, Encoding encoding)
        {
            m_input = input;
            m_reader = new BinaryReader(m_input, encoding, true);
            m_encoding = encoding;

            // Magic has already been read.
            m_input.Seek(21, SeekOrigin.Begin);

            m_input.Seek(2, SeekOrigin.Current); // Ignore 0x1A 0x00 (validate these bytes?)

            uint version = m_reader.ReadUInt32();
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

            int idx = s.IndexOf('\0');

            if (idx > -1)
                s = s.Substring(0, idx);

            return s;
        }

        public Node? ReadNode()
        {
            long startPos = m_input.Position;

            uint endOffset = m_reader.ReadUInt32();   // Offset to end of file?
            uint numProps = m_reader.ReadUInt32();    // Count of properties
            uint propListLen = m_reader.ReadUInt32(); // Length of properties in bytes

            byte nameLen = m_reader.ReadByte();

            if (endOffset == 0 && nameLen == 0)
                return null;

            string name = ReadString(nameLen);

            var properties = new List<Property>((int)numProps);
            var children = new List<Node>();

            for (int i = 0; i < numProps; ++i)
                properties.Add(ReadProperty());

            // Start reading nested nodes until "null"
            while (m_input.Position < endOffset)
            {
                var nested = ReadNode();

                if (nested == null)
                    break;

                children.Add(nested);
            }

            //long resPos = m_input.Position;
            //long len = resPos - startPos;

            var result = new Node
            {
                Name = name,
                Children = children.ToLookup(n => n.Name, n => n),
                Properties = properties
            };

            return result;
        }

        private Property ReadProperty()
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

            return new Property
            {
                Type = type,
                Data = data
            };
        }

        private byte[] Decompress(byte[] data)
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
