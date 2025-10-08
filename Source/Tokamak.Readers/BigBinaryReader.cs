using System;
using System.IO;
using System.Text;

namespace Tokamak.Readers
{
    public class BigBinaryReader : BinaryReader
    {
        public BigBinaryReader(Stream input, Encoding? encoding = null, bool leaveOpen = false)
            : base(input, encoding ?? Encoding.UTF8, leaveOpen)
        {
        }

        private byte[] EndianReadBytes(int length)
        {
            byte[] bytes = BinaryReadEx.ReadExactly(this, length);

            if (BitConverter.IsLittleEndian)
                Array.Reverse(bytes);

            return bytes;
        }

        public override short ReadInt16() =>
            BitConverter.ToInt16(EndianReadBytes(2), 0);

        public override int ReadInt32() =>
            BitConverter.ToInt32(EndianReadBytes(4), 0);

        public override long ReadInt64() =>
            BitConverter.ToInt64(EndianReadBytes(8), 0);

        public override ushort ReadUInt16() =>
            BitConverter.ToUInt16(EndianReadBytes(2), 0);

        public override uint ReadUInt32() =>
            BitConverter.ToUInt32(EndianReadBytes(4), 0);

        public override ulong ReadUInt64() =>
            BitConverter.ToUInt64(EndianReadBytes(8), 0);
    }
}
