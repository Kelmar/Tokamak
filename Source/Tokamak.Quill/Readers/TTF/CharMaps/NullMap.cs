using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tokamak.Quill.Readers.TTF.CharMaps
{
    /// <summary>
    /// Always maps to the unknown character glyph.
    /// </summary>
    /// <remarks>
    /// Should not use this intance unless we don't know the type of glyph mapping we have for the font.
    /// </remarks>
    internal class NullMap : ICharacterMapper
    {
        public int MapChar(char c)
        {
            return 0;
        }
    }
}
