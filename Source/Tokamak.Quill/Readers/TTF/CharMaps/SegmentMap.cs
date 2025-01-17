using System;
using System.IO;
using System.Linq;

namespace Tokamak.Quill.Readers.TTF.CharMaps
{
    internal class SegmentMap : ICharacterMapper
    {
        private readonly int[] m_startCodes;
        private readonly int[] m_endCodes;
        private readonly int[] m_idDeltas;
        private readonly int[] m_rangeOffsets;

        private readonly int[] m_glyphIds;

        public SegmentMap(ParseState state)
        {
            long start = state.Input.Position;

            int length = state.ReadUInt16(); // In bytes
            int language = state.ReadUInt16(); // Blah

            int segmentCount = state.ReadUInt16() >> 1; // Reading segCountX2

            /*
             * Docs suggest the next 6 bytes were for helping with building binary search trees.
             * 
             * Docs also suggest from Microsoft that these values should no longer be used
             * when reading from fonts, but should still be provided for older implementations.
             */

            state.Input.Seek(6, SeekOrigin.Current);

            m_endCodes = state.ReadUShorts(segmentCount).Select(i => (int)i).ToArray();

            state.Input.Seek(2, SeekOrigin.Current); // Skip reserved ushort

            m_startCodes = state.ReadUShorts(segmentCount).Select(i => (int)i).ToArray();
            m_idDeltas = state.ReadUShorts(segmentCount).Select(i => (int)i).ToArray();
            m_rangeOffsets = state.ReadUShorts(segmentCount).Select(i => (int)i).ToArray();

            int glyphIdCount = (int)(state.Input.Position - start) / 2;

            m_glyphIds = state.ReadUShorts(glyphIdCount).Select(i => (int)i).ToArray();
        }

        public int MapChar(char c)
        {
            if (c > UInt16.MaxValue)
                return 0;

            int i = Array.BinarySearch(m_endCodes, c);
            i = i < 0 ? ~i : i;

            if (i >= m_endCodes.Length || m_startCodes[i] > c)
                return 0; // Character not mapped

            if (m_rangeOffsets[i] == 0)
                return (m_idDeltas[i] + c) & 0xFFFF; // Modulo by 65,536
            else
            {
                /*
                 * This is what is documented on MS's page about TrueType/OpenType:
                 * 
                 * Docs state:
                 * | If the idRangeOffset value for the segment is not 0, the mapping
                 * | of character codes relies on glyphIdArray. The character code offset
                 * | from startCode is added to the idRangeOffset value. This sum is used
                 * | as an offset from the current location within idRangeOffset itself
                 * | to index out the correct glyphIdArray value. This obscure indexing
                 * | trick works because glyphIdArray immediately follows idRangeOffset
                 * | in the font file. The C expression that yields the glyph index is:
                 * | 
                 * | glyphId = *(idRangeOffset[i] / 2
                 * |     + (c - startCode[i])
                 * |     + &idRangeOffset[i])
                 * 
                 * So effectively this code is depdendent on a deliberate buffer overrun
                 * to make it work.... Sorry Microsoft this "obsecure trick" is the
                 * _EXACT_ sort of thing hackers take advantage of to hack a system....
                 */

                int index = (m_rangeOffsets[i] >> 1) + (c - m_startCodes[i]) - m_rangeOffsets.Length + i;

                if (index < 0 || index >= m_glyphIds.Length)
                {
                    // Seems to fail on Arial.ttf with Cyrillic character 0x0468 (1128)

                    // At this point I have to assume we've just lost the plot.
                    // Thanks for your "briliant" design Microsoft.....
                    return 0; 
                }

                return m_glyphIds[index];
            }
        }
    }
}
