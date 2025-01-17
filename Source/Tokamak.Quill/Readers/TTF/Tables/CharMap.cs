using System;
using System.Collections.Generic;
using System.Linq;

using Tokamak.Quill.Readers.TTF.CharMaps;

namespace Tokamak.Quill.Readers.TTF.Tables
{
    internal static class CharMap
    {
        private class EncodingRecord
        {
            public PlatformId Platform { get; set; }

            public ushort EncodingId { get; set; }

            public long Offset { get; set; }
        }

        private static EncodingRecord ReadEncodingRecord(ParseState state)
        {
            return new EncodingRecord
            {
                Platform = (PlatformId)state.ReadUInt16(),
                EncodingId = state.ReadUInt16(),
                Offset = state.ReadUInt32()
            };
        }

        private static void ReadSubtable(ParseState state, EncodingRecord encoding)
        {
            long saveSpot = state.Input.Position;

            try
            {
                state.Input.Position = encoding.Offset;

                ushort format = state.ReadUInt16();

                switch (format)
                {
#if false
                case 0: // Byte encoding table
                    break;

                case 2: // High byte mapping through table
                    break;
#endif

                case 4: // Segment mapping to delta values
                    state.CharMapper = new SegmentMap(state);
                    break;
#if false

                case 6: // Trimmed table mapping
                    break;

                case 8: // Mixed 16-bit and 32-bit coverage
                    break;

                case 10: // Trimmed array
                    break;

                case 12: // Segmented coverage
                    break;

                case 13: // Many-to-one range mappings
                    break;

                case 14: // Unicode variation sequences
                    break;
#endif

                default:
                    throw new NotSupportedException($"Unknown subtable format {format}");
                }
            }
            finally
            {
                state.Input.Position = saveSpot;
            }
        }

        public static void Load(ParseState state)
        {
            TableEntry entry = state.JumpToEntryOrFail("cmap");

            ushort cmapVersion = state.ReadUInt16();
            ushort numTables = state.ReadUInt16();

            if (cmapVersion != 0)
                throw new NotSupportedException($"cmap version {cmapVersion} not supported.");

            var encodings = new List<EncodingRecord>(numTables);

            for (int i = 0; i < numTables; ++i)
            {
                var rec = ReadEncodingRecord(state);
                rec.Offset += entry.Offset;
                encodings.Add(rec);
            }

            // For our purposes we're just going to read the Windows info.
            var encoding = encodings
                .Where(e =>
                    e.Platform == PlatformId.Windows &&
                    (e.EncodingId == 1 || e.EncodingId == 10)
                )
                .OrderByDescending(e => e.EncodingId)
                .FirstOrDefault();

            if (encoding == null)
                throw new NotSupportedException("Font does not have any unicode support.");

            ReadSubtable(state, encoding);
        }
    }
}
