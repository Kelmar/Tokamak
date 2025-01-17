using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Numerics;

using Tokamak.Mathematics;

namespace Tokamak.Quill.Readers.TTF.Tables
{
    internal static class GlyphTable
    {
        [Flags]
        enum SimpleGlyphFlags : byte
        {
            OnCurvePoint = 0x01,
            XShortVector = 0x02,
            YShortVector = 0x04,
            Repeat = 0x08,
            XIsSameOrPositive = 0x10,
            YIsSameOrPositive = 0x20,
            OverlapSimple = 0x40,
            Reserved = 0x80
        }

        [Flags]
        enum CompositeGlyphFlags : UInt16
        {
            Arg1And2AreWords = 0x0001,
            ArgsAreXYValues = 0x0002,
            RoundXYToGrid = 0x0004,
            WeHaveAScale = 0x0008,
            MoreComponents = 0x0020,
            WeHaveAnXAndYScale = 0x0040,
            WeHaveATwoByTwo = 0x0080,
            WeHaveInstructions = 0x0100,
            UseMyMetrix = 0x0200,
            OverlapCompound = 0x0400,
            ScaledComponentOffset = 0x0800,
            UnscaledComponentOffset = 0x1000,
            Reserved = 0xE010
        }

        #region Simple Glyph Reading

        private static int ReadSimpleCoordinate(ParseState state, bool isByte, bool isSameOrPositive)
        {
            int value;

            if (isByte)
            {
                value = state.SafeReadByte();

                if (!isSameOrPositive)
                    value = -value;
            }
            else
            {
                if (isSameOrPositive)
                    value = 0;
                else
                    value = state.ReadInt16();
            }

            return value;
        }

        private static IEnumerable<SimpleGlyphFlags> ReadSimpleGlyphFlags(ParseState state, int pointCount)
        {
            int index = 0;

            while (index < pointCount)
            {
                var flags = (SimpleGlyphFlags)state.SafeReadByte();
                yield return flags;

                ++index;

                if (flags.HasFlag(SimpleGlyphFlags.Repeat))
                {
                    byte count = state.SafeReadByte();

                    if ((count + index) > pointCount)
                    {
                        throw new FontFileException("Repeat flag exceeds point count.")
                        {
                            Data =
                            {
                                ["type"] = "SimpleGlyph",
                                ["index"] = index,
                                ["max"] = pointCount,
                                ["overflow"] = count + index
                            }
                        };
                    }

                    for (int i = 0; i < count; ++i)
                        yield return flags;

                    index += count;
                }
            }
        }

        private static TTFSimpleGlyph ReadSimpleGlyph(ParseState state, int numberOfContours, int index)
        {
            UInt16[] endPtsOfContours = state.ReadUShorts(numberOfContours);

            UInt16 instructionLength = state.ReadUInt16();

            byte[] instructions = state.ReadBytes(instructionLength);

            int pointCount = endPtsOfContours[numberOfContours - 1] + 1;

#if false
            // This is just a sanity check while we figure out what we're doing....
            if (m_maxPoints > 0)
                Debug.Assert(pointCount <= m_maxPoints, "Glyph exceeds max points for font.");
#endif

            var flagList = ReadSimpleGlyphFlags(state, pointCount).ToList();

            Vector2[] points = new Vector2[pointCount];
            int lastValue = 0;

            // Read X values
            for (int i = 0; i < pointCount; ++i)
            {
                SimpleGlyphFlags flags = flagList[i];

                int delta = ReadSimpleCoordinate(
                    state,
                    flags.HasFlag(SimpleGlyphFlags.XShortVector),
                    flags.HasFlag(SimpleGlyphFlags.XIsSameOrPositive)
                );

                lastValue += delta;
                points[i].X = lastValue;
            }

            lastValue = 0;

            // Read Y values
            for (int i = 0; i < pointCount; ++i)
            {
                SimpleGlyphFlags flags = flagList[i];

                int delta = ReadSimpleCoordinate(
                    state,
                    flags.HasFlag(SimpleGlyphFlags.YShortVector),
                    flags.HasFlag(SimpleGlyphFlags.YIsSameOrPositive)
                );

                lastValue += delta;
                points[i].Y = lastValue;
            }

            var rval = new TTFSimpleGlyph
            {
                Index = index,
                Instructions = instructions.ToList()
            };

            // Rebuild into list of bits need for translation to internal format.

            TTFSimpleGlyph.Contour current = null;

            for (int i = 0, j = 0; i < pointCount; ++i)
            {
                Debug.Assert(j < endPtsOfContours.Length);

                if (current == null)
                    current = new TTFSimpleGlyph.Contour();

                current.Points.Add(new TTFSimpleGlyph.Point
                {
                    Value = points[i],
                    OnCurve = flagList[i].HasFlag(SimpleGlyphFlags.OnCurvePoint),
                });

                if (endPtsOfContours[j] == i)
                {
                    rval.Contours.Add(current);
                    current = null;
                    ++j;
                }
            }

            return rval;
        }

        #endregion

        private static Vector2 ReadGlyphVector(ParseState state, CompositeGlyphFlags flags)
        {
            int x = 0;
            int y = 0;

            if (flags.HasFlag(CompositeGlyphFlags.Arg1And2AreWords))
            {
                if (flags.HasFlag(CompositeGlyphFlags.ArgsAreXYValues))
                {
                    x = state.ReadInt16();
                    y = state.ReadInt16();
                }
                else
                {
                    x = state.ReadUInt16();
                    y = state.ReadUInt16();
                }
            }
            else
            {
                if (flags.HasFlag(CompositeGlyphFlags.ArgsAreXYValues))
                {
                    x = y = state.ReadInt16();
                }
                else
                {
                    x = y = state.ReadUInt16();
                }
            }

            return new Vector2(x, y);
        }

        private static TTFCompoundGlyph ReadCompositeGlyph(ParseState state, int index)
        {
            CompositeGlyphFlags flags;

            var rval = new TTFCompoundGlyph
            {
                Index = index
            };

            do
            {
                flags = (CompositeGlyphFlags)state.ReadUInt16();

                //Debug.Assert((flags & CompositeGlyphFlags.Reserved) == 0, "Invalid composite flags!");

                ChildGlyphInfo child = new ChildGlyphInfo
                {
                    Index = state.ReadUInt16(),
                    Transform = Matrix3x2.Identity
                };

                // Docs state some of the flags will dictate how we use this.
                Vector2 vector = ReadGlyphVector(state, flags);

                Matrix3x2 translate = Matrix3x2.CreateTranslation(vector);
                Matrix3x2 scale = Matrix3x2.Identity;

                if (flags.HasFlag(CompositeGlyphFlags.WeHaveAScale))
                {
                    scale = Matrix3x2.CreateScale(state.ReadF2Dot14());
                }
                else if (flags.HasFlag(CompositeGlyphFlags.WeHaveAnXAndYScale))
                {
                    scale.M11 = state.ReadF2Dot14();
                    scale.M22 = state.ReadF2Dot14();
                }
                else if (flags.HasFlag(CompositeGlyphFlags.WeHaveATwoByTwo))
                {
                    // 2x2 transformation matrix
                    scale.M11 = state.ReadF2Dot14(); // ScaleX
                    scale.M12 = state.ReadF2Dot14();
                    scale.M21 = state.ReadF2Dot14();
                    scale.M22 = state.ReadF2Dot14(); // ScaleY
                }

                child.Transform *= scale;
                child.Transform *= translate;

                rval.Children.Add(child);
            } while (flags.HasFlag(CompositeGlyphFlags.MoreComponents));

            if (flags.HasFlag(CompositeGlyphFlags.WeHaveInstructions))
            {
                int count = state.ReadUInt16();
                rval.Instructions = state.ReadBytes(count).ToList();
            }

            return rval;
        }

        private static ITTFGlyph ReadSingleGlyph(ParseState state, long tableOffset, int index)
        {
            long len = state.GlyphOffsets[index + 1] - state.GlyphOffsets[index];

            if (len == 0)
                return null; // No details for this glyph

            using var ctx = state.ReadContext(tableOffset + state.GlyphOffsets[index]);

            // Read header info
            int numberOfContours = state.ReadInt16();

            Vector2 min = new Vector2(state.ReadInt16(), state.ReadInt16());
            Vector2 max = new Vector2(state.ReadInt16(), state.ReadInt16());

            ITTFGlyph rval;

            if (numberOfContours >= 0)
                rval = ReadSimpleGlyph(state, numberOfContours, index);
            else if (numberOfContours == -1)
                rval = ReadCompositeGlyph(state, index);
            else
            {
                throw new FontFileException($"Invalid number of contours.")
                {
                    Data =
                    {
                        ["index"] = index,
                        ["value"] = numberOfContours
                    }
                };
            }

            rval.Bounds = new RectF(min, max - min);

            return rval;
        }

        private static IEnumerable<ITTFGlyph> Enumerate(ParseState state, TableEntry entry)
        {
            ITTFGlyph last = null;

            for (int index = 0; index < state.GlyphCount; ++index)
            {
                ITTFGlyph g = ReadSingleGlyph(state, entry.Offset, index);

                if (g == null)
                {
                    if (last == null)
                        throw new FontFileException($"Glyph index 0 not defined.");

                    yield return last;
                }
                else
                {
                    last = g;
                    yield return g;
                }
            }
        }

        public static void Load(ParseState state)
        {
            Debug.Assert(state.GlyphOffsets != null, "LocationMap not called.");
            Debug.Assert(state.GlyphOffsets.Count == state.GlyphCount + 1, "Invalid data from LocationMap reader.");

            TableEntry entry = state.JumpToEntryOrFail("glyf");

            state.Glyphs = Enumerate(state, entry).ToList();
        }
    }
}
