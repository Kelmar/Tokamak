using System.Collections.Generic;

using Tokamak.Mathematics;

namespace Tokamak.Quill.Readers.TTF
{
    internal interface ITTFGlyph
    {
        public int Index { get; }

        public RectF Bounds { get; set; }

        public List<byte> Instructions { get; }
    }
}
