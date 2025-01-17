﻿using System;
using System.Text;

namespace Tokamak.Quill.Readers.TTF.Tables
{
    internal static class NameTable
    {
        private class NameRecord
        {
            /// <summary>
            /// This is PlatformID specific.
            /// </summary>
            public required int LanguageId { get; set; }

            public required NameId NameId { get; set; }

            public required string Name { get; set; }
        }

        private static NameRecord? ReadNameRecord(ParseState state, long storePosition)
        {
            var platform = (PlatformId)state.ReadUInt16();
            UInt16 encodingId = state.ReadUInt16();

            Encoding? encoding = EncodingMap.FindEncodingFor(platform, encodingId);

            if (encoding == null)
                return null;

            int length = state.ReadUInt16();
            int stringOffset = state.ReadUInt16();

            using var ctx = state.ReadContext(storePosition + stringOffset);

            byte[] b = state.ReadBytes(length);

            return new NameRecord
            {
                LanguageId = state.ReadUInt16(),
                NameId = (NameId)state.ReadUInt16(),
                Name = encoding.GetString(b)
            };
        }

        public static void Load(ParseState state)
        {
            TableEntry entry = state.JumpToEntryOrFail("name");

            int version = state.ReadUInt16();
            int count = state.ReadUInt16();
            long storeOffset = state.ReadUInt16();

            long storePosition = entry.Offset + storeOffset;

            for (int i = 0; i < count; ++i)
            {
                var nameRecord = ReadNameRecord(state, storePosition);

                if (nameRecord == null)
                    continue; // Unsupported name

                state.Names[nameRecord.NameId] = nameRecord.Name;
            }
        }
    }
}
