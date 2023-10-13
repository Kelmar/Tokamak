using System;
using System.Diagnostics;
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

        public BinaryFormatReader(Stream input)
        {
            m_input = input;

            // Magic has already been read.
            m_input.Seek(21, SeekOrigin.Begin);

            m_input.Seek(2, SeekOrigin.Current); // Ignore 0x1A 0x00 (validate these bytes?)

            uint version = ReadUInt32();
        }

        private byte[] ReadExactly(int length)
        {
            byte[] buffer = new byte[length];
            int rd = m_input.Read(buffer);

            if (rd != length)
                throw new Exception("Unexpected end of file.");

            return buffer;
        }

        private string ReadString(int length)
        {
            byte[] data = ReadExactly(length);
            string s = Encoding.UTF8.GetString(data); // ASCII encoded?

            // The format and .NET are quite happy to accept nulls in this string.
            // I susspect these are just being written as flat buffers by Blender.
            // Trim at null characters.

            int idx = s.IndexOf('\0');

            if (idx > -1)
                s = s.Substring(0, idx);

            return s;
        }

        private byte ReadByte()
        {
            int i = m_input.ReadByte();

            if (i == -1)
                throw new Exception("Unexpected end of file.");

            return (byte)i;
        }

        private short ReadInt16()
        {
            byte[] buffer = ReadExactly(2);
            return BitConverter.ToInt16(buffer, 0);
        }

        private uint ReadUInt32()
        {
            byte[] buffer = ReadExactly(4);
            return BitConverter.ToUInt32(buffer, 0);
        }

        private int ReadInt32()
        {
            byte[] buffer = ReadExactly(4);
            return BitConverter.ToInt32(buffer, 0);
        }

        private long ReadInt64()
        {
            byte[] buffer = ReadExactly(8);
            return BitConverter.ToInt64(buffer, 0);
        }

        private float ReadFloat()
        {
            byte[] buffer = ReadExactly(4);
            return BitConverter.ToSingle(buffer, 0);
        }

        private double ReadDouble()
        {
            byte[] buffer = ReadExactly(8);
            return BitConverter.ToDouble(buffer, 0);
        }

        public Node ReadNode()
        {
            long startPos = m_input.Position;

            uint endOffset = ReadUInt32();      // Offset to end of file?
            uint numProps = ReadUInt32();       // Count of properties
            uint propListLen = ReadUInt32();    // Length of properties in bytes

            byte nameLen = ReadByte();

            if (endOffset == 0 && nameLen == 0)
                return null;

            var rval = new Node();
            rval.Name = ReadString(nameLen);

            for (int i = 0; i < numProps; ++i)
            {
                var prop = ReadProperty();
                rval.Properties.Add(prop);
            }

            if (m_input.Position < endOffset)
            {
                // Start reading nested nodes until "null"
                for (; ; )
                {
                    var nested = ReadNode();

                    if (nested == null)
                        break;

                    rval.AddChild(nested);
                }
            }

            //long resPos = m_input.Position;
            //long len = resPos - startPos;

            return rval;
        }

        private Property ReadProperty()
        {
            char c = (char)ReadByte();

            var rval = new Property();
            rval.Type = (PropertyType)c;

            switch (rval.Type)
            {
            case PropertyType.SignedShort: rval.Data = ReadInt16(); break;
            case PropertyType.Boolean: rval.Data = ReadByte(); break;
            case PropertyType.SignedInt: rval.Data = ReadInt32(); break;
            case PropertyType.Float: rval.Data = ReadFloat(); break;
            case PropertyType.Double: rval.Data = ReadDouble(); break;
            case PropertyType.SignedLong: rval.Data = ReadInt64(); break;
            case PropertyType.FloatArray: rval.Data = ReadFloatArray(); break;
            case PropertyType.DoubleArray: rval.Data = ReadDoubleArray(); break;
            case PropertyType.LongArray: rval.Data = ReadLongArray(); break;
            case PropertyType.IntArray: rval.Data = ReadIntArray(); break;
            case PropertyType.BoolArray: rval.Data = ReadBoolArray(); break;
            case PropertyType.String: rval.Data = ReadPropertyString(); break;
            case PropertyType.RawBinary: rval.Data = ReadPropertyRaw(); break;

            default:
                throw new Exception($"Unknown property type '{c}'");
            }

            return rval;
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
            int length = (int)ReadUInt32();
            bool compressed = ReadUInt32() == 1;
            int physicalLength = (int)ReadUInt32();

            int itemSize = Marshal.SizeOf<T>();
            int recordSize = compressed ? physicalLength : (length * itemSize);

            byte[] data = ReadExactly(recordSize);

            if (compressed)
            {
                data = Decompress(data);

                Debug.Assert(data.Length / itemSize == length);
                Debug.Assert(data.Length % itemSize == 0);
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
            int length = (int)ReadUInt32();
            byte[] data = ReadExactly(length);

            string s = Encoding.UTF8.GetString(data); // ASCII encoded?

            // The format and .NET are quite happy to accept nulls in this string.
            // I susspect these are just being written as flat buffers by Blender.
            // Trim at null characters.
            int idx = s.IndexOf('\0');

            if (idx > 0)
                s = s.Substring(0, idx);

            return s;
        }

        private byte[] ReadPropertyRaw()
        {
            int length = (int)ReadUInt32();
            return ReadExactly(length);
        }
    }
}
