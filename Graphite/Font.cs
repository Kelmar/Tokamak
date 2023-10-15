using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;

using FreeTypeWrapper;

using Tokamak;
using Tokamak.Buffer;
using Tokamak.Formats;
using Tokamak.Mathematics;

namespace Graphite
{
    public class Font : IDisposable
    {
        private const int CACHE_SHEET_SIZE = 512;

        private readonly Platform m_device;
        private readonly FTFace m_face;

        private readonly List<ITextureObject> m_cacheSheets = new List<ITextureObject>(4);
        private readonly IDictionary<char, Glyph> m_glyphs = new Dictionary<char, Glyph>();

        private ITextureObject m_currentSheet;
        private Point m_sheetIndex;

        internal Font(Platform device, FTFace face)
        {
            m_device = device;
            m_face = face;

            AddSheet();

            m_currentSheet.Refresh();
        }

        public void Dispose()
        {
            foreach (var sheet in m_cacheSheets)
                sheet.Dispose();

            m_face.Dispose();
        }

        public FTFace FTFace => m_face;

        public bool HasKerning => m_face.HasKerning;

        public string FamilyName => m_face.FamilyName;

        public string StyleName => m_face.StyleName;

        public int LineSpacing => m_face.LineSpacing;

        public float Size => m_face.Size;

        private void AddSheet()
        {
            m_currentSheet = m_device.GetTextureObject(PixelFormat.FormatA8, new Point(CACHE_SHEET_SIZE, CACHE_SHEET_SIZE));
            m_currentSheet.Bitmap.Clear();
            m_cacheSheets.Add(m_currentSheet);
            m_sheetIndex = new Point(0, m_face.LineSpacing);
        }

        internal ITextureObject GetSheet(int index) => m_cacheSheets[index];

        public void RefreshSheets()
        {
            var refreshes = m_cacheSheets.Where(cs => cs.Bitmap.Dirty);

            foreach (var cs in refreshes)
            {
                cs.Refresh();
                cs.Bitmap.NotDirty();
            }
        }

        public float GetKerning(char l, char r)
        {
            return HasKerning ? m_face.GetKerning(l, r) : 0;
        }

        public Glyph GetGlyph(char c)
        {
            if (!m_glyphs.TryGetValue(c, out Glyph rval))
            {
                // Get the glyph info
                m_face.SetGlyph(c);

                var metrics = m_face.GetCurrentGlyphMetrics();

                if (m_sheetIndex.X + m_face.Glyph.bitmap.width >= m_currentSheet.Size.X)
                {
                    // Wrap to next line....
                    m_sheetIndex.X  = 0;
                    m_sheetIndex.Y += m_face.LineSpacing;

                    if (m_sheetIndex.Y >= m_currentSheet.Size.Y)
                        AddSheet(); // Need a new sheet!
                }

                Point size = new Point((int)m_face.Glyph.bitmap.width, (int)m_face.Glyph.bitmap.rows);

                Vector2 tl = new Point(m_sheetIndex.X + m_face.Glyph.bitmap_left, m_sheetIndex.Y - m_face.Glyph.bitmap_top);
                Vector2 br = tl + size;

                tl.X /= m_currentSheet.Size.X;
                br.X /= m_currentSheet.Size.X;

                tl.Y /= m_currentSheet.Size.Y;
                br.Y /= m_currentSheet.Size.Y;

                rval = new Glyph
                {
                    Char = c,
                    SheetNumber = m_cacheSheets.Count - 1,
                    Size = size,
                    TopLeftUV = tl,
                    BotRightUV = br,
                    Advance = metrics.Advance,
                    Bearing = new Point((int)m_face.Glyph.bitmap_left, (int)m_face.Glyph.bitmap_top)
                };

                if (rval.Bearing == Point.Zero)
                {
                    rval.Bearing = new Point((int)Math.Ceiling(metrics.Advance.X), LineSpacing);
                }

                m_face.RenderGlyph(m_currentSheet.Bitmap, m_sheetIndex);

                m_glyphs[c] = rval;

                m_sheetIndex.X += size.X + 2; // Give a tiny bit of extra padding just incase.

                m_currentSheet.Refresh();
            }

            return rval;
        }

        public Point MeasureString(string text)
        {
            Point rval = Point.Zero;

            foreach (char c in text)
            {
                var metrics = GetGlyph(c);

                rval.X += metrics.Size.X;
                rval.Y = Math.Max(metrics.Size.Y, rval.Y);
            }

            return rval;
        }
    }
}
