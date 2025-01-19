using System.Collections.Generic;
using System.Text;

namespace Tokamak.Quill.Readers.TTF
{
    internal static class EncodingMap
    {
        private static readonly Dictionary<int, Encoding> s_unicodeEncodings = new()
        {
            [0] = Encoding.BigEndianUnicode, // Unicode 1.0 (deprecated)
            [1] = Encoding.BigEndianUnicode, // Unicode 1.1 (deprecated)
            [2] = Encoding.BigEndianUnicode, // ISO/IEC 10646 (deprecated)
            [3] = Encoding.BigEndianUnicode, // Unicode 2.0 (BMP)
            [4] = Encoding.BigEndianUnicode, // Unicode 2.0 (Full)
        };

        private static readonly Dictionary<int, Encoding> s_macintoshEncodings = new()
        {
            [0] = Encoding.ASCII, // Roman
            //[1 ] = Encoding.???, // Japanese
            //[2 ] = Encoding.???, // Chinese (Traditional)
            //[3 ] = Encoding.???, // Korean
            //[4 ] = Encoding.???, // Arabic
            //[5 ] = Encoding.???, // Hebrew
            //[6 ] = Encoding.???, // Greek
            //[7 ] = Encoding.???, // Russian
            //[8 ] = Encoding.???, // RSymbol
            //[9 ] = Encoding.???, // Devanagari
            //[10] = Encoding.???, // Gurmukhi
            //[11] = Encoding.???, // Gujarati
            //[12] = Encoding.???, // Odia
            //[13] = Encoding.???, // Bangla
            //[14] = Encoding.???, // Tamil
            //[15] = Encoding.???, // Telugu
            //[16] = Encoding.???, // Kannada
            //[17] = Encoding.???, // Malayalm
            //[18] = Encoding.???, // Sinhalese
            //[19] = Encoding.???, // Burmese
            //[20] = Encoding.???, // Khmer
            //[21] = Encoding.???, // Thai
            //[22] = Encoding.???, // Laotian
            //[23] = Encoding.???, // Georgian
            //[24] = Encoding.???, // Armenian
            //[25] = Encoding.???, // Chinese (Simplified)
            //[26] = Encoding.???, // Tibetan
            //[27] = Encoding.???, // Mongolian
            //[28] = Encoding.???, // Geez
            //[29] = Encoding.???, // Slavic
            //[30] = Encoding.???, // Sindhi
            //[32] = Encoding.???, // Uninterpreted
        };

        private static readonly Dictionary<int, Encoding> s_windowsEncodings = new()
        {
            [0] = Encoding.ASCII,              // Symbol
            [1] = Encoding.BigEndianUnicode,   // Unicode (BMP)
            //[2 ] = Encoding.ShiftJIS,
            //[3 ] = Encoding.PRC,
            //[4 ] = Encoding.Big5,
            //[5 ] = Encoding.Wansung,
            //[6 ] = Encoding.Johab,
            // 7 - 9 == Reserved
            [10] = Encoding.BigEndianUnicode,   // Unicode (Full)
        };

        private static readonly Dictionary<PlatformId, Dictionary<int, Encoding>> s_platformEncodingMap = new();

        static EncodingMap()
        {
            s_platformEncodingMap[PlatformId.Unicode] = s_unicodeEncodings;
            s_platformEncodingMap[PlatformId.Macintosh] = s_macintoshEncodings;
            s_platformEncodingMap[PlatformId.Windows] = s_windowsEncodings;
        }

        public static Encoding? FindEncodingFor(PlatformId platformId, int encodingId)
        {
            if (!s_platformEncodingMap.TryGetValue(platformId, out var encodingMap))
                return null; // Unsuported platform

            if (!encodingMap.TryGetValue(encodingId, out Encoding? encoding))
                return null; // Unsupported encoding

            return encoding;
        }
    }
}
