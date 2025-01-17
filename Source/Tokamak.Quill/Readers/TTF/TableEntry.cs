using System;
using System.Text;

namespace Tokamak.Quill.Readers.TTF
{
    internal class TableEntry
    {
        public string Tag { get; set; } = String.Empty;

        public UInt32 CheckSum { get; set; }

        public long Offset { get; set; }

        public long Length { get; set; }

        public override string ToString()
        {
            return $"{Tag} -> {Offset}";
        }

        public static TableEntry LoadFrom(ParseState state)
        {
            byte[] tagBytes = state.ReadBytes(4);

            string tag = Encoding.ASCII.GetString(tagBytes).Trim();

            return new TableEntry
            {
                Tag = tag,
                CheckSum = state.ReadUInt32(),
                Offset = state.ReadUInt32(),
                Length = state.ReadUInt32()
            };
        }
    }
}
