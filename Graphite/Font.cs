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
            int pitch = width * 4;
            int size = height * pitch;
            int offset = 0;

            var bitmap = new byte[size];

            m_face.RenderGlyph(c, (data, row) =>
            {
                int toCopy = Math.Min(pitch, data.Length);
                Array.Copy(data, 0, bitmap, offset, toCopy);
                offset += pitch;
            });

            var rval = m_device.GetTextureObject(
                Tokamak.Formats.PixelFormat.FormatR8G8B8A8, 
                new Point(width, height));

            rval.Set(bitmap);

            return rval;
        }
    }
}
