using System.IO;

namespace Tokamak.Readers
{
    public static class BinaryReadEx
    {
        private const string EOF_ERROR_MSG = "Unexpected end of stream reached.";

        public static byte[] ReadExactly(this BinaryReader reader, int length)
        {
            byte[] b = reader.ReadBytes(length);

            if (b.Length < length)
                throw new EndOfStreamException(EOF_ERROR_MSG);

            return b;
        }
    }
}
