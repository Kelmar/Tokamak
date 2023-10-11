using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using FreeTypeWrapper;

using Tokamak;
using Tokamak.Buffer;
using Tokamak.Mathematics;

namespace Graphite
{
    public class Font : IDisposable
    {
        private readonly Device m_device;
        private readonly FTFace m_face;

        internal Font(Device device, FTFace face)
        {
            m_device = device;
            m_face = face;
        }

        public void Dispose()
        {
            m_face.Dispose();
        }

        public ITextureObject DrawGlyph(char c)
        {
            int height = (int)Math.Ceiling(m_face.FontExtents.MaxBitmap.Y);
            int width = (int)Math.Ceiling(m_face.FontExtents.MaxBitmap.X);

            var size = new Point(width, height);

            var bitmap = new Bitmap(size, Tokamak.Formats.PixelFormat.FormatA8);

            m_face.RenderGlyph(c, bitmap);

            var rval = m_device.GetTextureObject(bitmap.Format, bitmap.Size);

            rval.Set(bitmap);

            return rval;
        }
    }
}
