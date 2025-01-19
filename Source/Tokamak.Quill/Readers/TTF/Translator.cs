using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;

using Tokamak.Mathematics;

namespace Tokamak.Quill.Readers.TTF
{
    /// <summary>
    /// Translates TTF data into internal format.
    /// </summary>
    internal class Translator
    {
        private const float POINTS_PER_INCH = 72;

        private readonly ParseState m_state;
        private readonly Dictionary<int, IGlyph> m_glyphs = new();

        // Recursive detection
        private readonly HashSet<int> m_processing = new();

        public Translator(ParseState state, in Point DPI)
        {
            m_state = state;
        }

        private IGlyph TranslateGlyph(ITTFGlyph glyph)
        {
            if (m_glyphs.ContainsKey(glyph.Index))
                return m_glyphs[glyph.Index];

            if (m_processing.Contains(glyph.Index))
                throw new FontFileException($"Cyclic glyph reference!");

            try
            {
                m_processing.Add(glyph.Index);

                IGlyph rval = glyph switch
                {
                    TTFSimpleGlyph simple => TranslateSimpleGlyph(simple),
                    TTFCompoundGlyph compound => TranslateCompoundGlyph(compound),
                    _ => throw new Exception("BUG: Unknown glyph type encountered!")
                };

                m_glyphs[glyph.Index] = rval;

                return rval;
            }
            finally
            {
                m_processing.Remove(glyph.Index);
            }
        }

        private IGlyph GetGlyph(int index)
        {
            IGlyph? rval;

            if (m_glyphs.TryGetValue(index, out rval))
                return rval;

            // Glyph could be an alias to another index.
            ITTFGlyph ttfGlyph = m_state.Glyphs[index];

            if (ttfGlyph.Index != index && m_glyphs.TryGetValue(ttfGlyph.Index, out rval))
                return rval;

            rval = TranslateGlyph(ttfGlyph);
            m_glyphs[rval.Index] = rval;

            return rval;
        }

        private IEnumerable<Segment> BuildSegments(TTFSimpleGlyph.Contour contour)
        {
            // Push the first point to the end to "close" the loop.
            var work = new List<TTFSimpleGlyph.Point>(contour.Points);
            work.Add(contour.Points[0]);

            Segment current = new Segment();

            TTFSimpleGlyph.Point? last = null;

            foreach (var point in work)
            {
                if (last != null)
                {
                    if (!last.OnCurve && !point.OnCurve)
                    {
                        // Implied on curve point
                        Vector2 implied = (last.Value + point.Value) / 2f;

                        current.Points.Add(implied);

                        //Debug.Assert(current.Points.Count > 2); // Hrm....

                        yield return current;

                        current = new Segment();
                        current.Points.Add(implied);
                    }

                    if (point.OnCurve)
                    {
                        current.Points.Add(point.Value);

                        yield return current;

                        current = new Segment();
                    }
                }

                current.Points.Add(point.Value);

                last = point;
            }

            if (current.Points.Count > 1)
                yield return current;
        }

        private IEnumerable<Loop> BuildLoops(TTFSimpleGlyph glyph)
        {
            foreach (var contour in glyph.Contours)
            {
                yield return new Loop()
                {
                    Segments = BuildSegments(contour).ToList()
                };
            }
        }

        private SimpleGlyph TranslateSimpleGlyph(TTFSimpleGlyph glyph)
        {
            return new SimpleGlyph
            {
                Index = glyph.Index,
                Bounds = glyph.Bounds,
                Loops = BuildLoops(glyph).ToList()
            };
        }

        private CompoundGlyph TranslateCompoundGlyph(TTFCompoundGlyph glyph)
        {
            var rval = new CompoundGlyph
            {
                Index = glyph.Index,
                Bounds = glyph.Bounds
            };

            foreach (var child in glyph.Children)
            {
                var childGlyph = GetGlyph(child.Index);

                rval.AddChild(childGlyph, child.Transform);
            }

            return rval;
        }

        private IEnumerable<IGlyph> TranslateGlyphs()
        {
            foreach (var glyph in m_state.Glyphs)
                yield return TranslateGlyph(glyph);
        }

        public Font Get(float pointSize, in Point DPI)
        {
            var glyphs = TranslateGlyphs();

            Vector2 scale = pointSize * DPI / (POINTS_PER_INCH * m_state.UnitsPerEm);

            return new Font(m_state.CharMapper)
            {
                FontId = m_state.NameSearch(NameId.FontId, NameId.Family, NameId.FontName),
                Family = m_state.NameSearch(NameId.Family, NameId.FontId, NameId.FontName),
                Subfamily = m_state.NameSearch(NameId.Subfamily),
                Glyphs = glyphs.ToList().AsReadOnly(),
                Points = pointSize,
                Scale = scale
            };
        }
    }
}
