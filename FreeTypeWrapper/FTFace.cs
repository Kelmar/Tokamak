using System;
using System.Diagnostics;
using System.Numerics;
using System.Runtime.InteropServices;

using FreeTypeSharp.Native;

using Tokamak.Buffer;

using static FreeTypeSharp.Native.FT;

namespace FreeTypeWrapper
{
    public unsafe class FTFace : IDisposable
    {
        // These aren't defined in the nuget package?
        private const int FT_STYLE_FLAG_ITALIC = 1 << 0;
        private const int FT_STYLE_FLAG_BOLD = 1 << 1;

        /// <summary>
        /// One "point" is 1/72th of an inch.
        /// </summary>
        public const float POINTS_PER_INCH = 72f;

        private readonly IntPtr m_handle;
        private readonly FT_FaceRec* m_faceRec;

        private bool m_disposed = false;

        internal FTFace(IntPtr handle, float size, in Vector2 dpi)
        {
            Debug.Assert(handle != IntPtr.Zero, "Invalid handle");

            m_handle = handle;
            m_faceRec = (FT_FaceRec*)m_handle;

            Size = size;
            DPI = dpi;
            //CharMap = charMap;

            //SafeExecute(() => FT_Set_Charmap(m_handle, CharMap));

            SetSize();

            FontExtents = new FontExtents
            {
                DPI = DPI,
                EmToPixel = (Size / POINTS_PER_INCH) * DPI.Y,
                AscenderEm = EmsPerUnit * FaceRecRaw.ascender,
                DescenderEm = EmsPerUnit * FaceRecRaw.descender,
                LineSpacingEm = EmsPerUnit * FaceRecRaw.size->metrics.height,
                UnderlinePositionEm = EmsPerUnit * FaceRecRaw.underline_position,
                UnderlineThicknessEm = EmsPerUnit * FaceRecRaw.underline_thickness,
                MinEm = EmsPerUnit * new Vector2(FaceRecRaw.bbox.xMin, FaceRecRaw.bbox.yMin),
                MaxEm = EmsPerUnit * new Vector2(FaceRecRaw.bbox.xMax, FaceRecRaw.bbox.yMax)
            };
        }

        protected virtual void Dispose(bool disposing)
        {
            if (disposing && m_handle != IntPtr.Zero)
            {
                // Maybe log this if there's a problem, but not much we can do if it fails....
                FT_Done_Face(m_handle);
            }

            m_disposed = true;
        }

        public void Dispose()
        {
            Dispose(true);
        }

        private void SafeExecute(Func<FT_Error> func)
        {
            if (m_disposed)
                throw new ObjectDisposedException(nameof(FTFace));

            FT_Error err = func();

            if (err != FT_Error.FT_Err_Ok)
                throw new FreeTypeException(err);
        }

        private void SetSize()
        {
            if (!IsScalable)
                throw new Exception($"Font {FamilyName} is not a scalable font!");

            float emToPixel = (Size / POINTS_PER_INCH) * DPI.Y;
            int size = (int)Math.Round(Size * 64); // Magic number 64?
            //int size = (int)Math.Round(Size * 640);

            SafeExecute(() => FT_Set_Char_Size(m_handle, 0, size, (uint)DPI.X, (uint)DPI.Y));
        }

        public Vector2 DPI { get; }

        public float Size { get; }

        //public int CharMap { get; }

        /// <summary>
        /// Get various sizing metrics for glyphs of this font.
        /// </summary>
        public FontExtents FontExtents { get; }

        /// <summary>
        /// Raw underlying FreeType FaceRec structure.
        /// </summary>
        /// <remarks>
        /// This record has pointers and may require unsafe code to access correctly.
        /// </remarks>
        public FT_FaceRec FaceRecRaw => *m_faceRec;

        /// <summary>
        /// Gets the current glyph slot.
        /// </summary>
        public FT_GlyphSlotRec CurrentGlyph => *FaceRecRaw.glyph;

        /// <summary>
        /// Indicates that the face contains outline glyphs.
        /// </summary>
        /// <remarks>
        /// This doesn't prvent bitmap strikes, i.e., a face can have both this and HasFixedSizes
        /// </remarks>
        public bool IsScalable => HasFlag(FT_FACE_FLAG_SCALABLE);

        /// <summary>
        /// Indicates that the face contains bitmap strikes.
        /// </summary>
        /// <remarks>
        /// This can appear in combination with IsScalable.
        /// </remarks>
        public bool HasFixedSizes => HasFlag(FT_FACE_FLAG_FIXED_SIZES);

        /// <summary>
        /// Indicates that the font face has color tables.
        /// </summary>
        public bool HasColor => HasFlag(FT_FACE_FLAG_COLOR);

        /// <summary>
        /// Indicates that the face contains kerning information.
        /// </summary>
        public bool HasKerning => HasFlag(FT_FACE_FLAG_KERNING);

        public bool IsBold => HasStyle(FT_STYLE_FLAG_BOLD);

        public bool IsItalic => HasStyle(FT_STYLE_FLAG_ITALIC);

        public float UnitsPerEm => FaceRecRaw.units_per_EM;

        public float EmsPerUnit => 1f / UnitsPerEm;

        public bool HasBitmapStrikes => (FaceRecRaw.num_fixed_sizes > 0) && (FaceRecRaw.available_sizes != null);

        /// <summary>
        /// Get the line spacing for this face in pixels.
        /// </summary>
        public int LineSpacing => (int)FaceRecRaw.size->metrics.height >> 6;

        public string FamilyName => Marshal.PtrToStringAnsi(FaceRecRaw.family_name);

        public string StyleName => Marshal.PtrToStringAnsi(FaceRecRaw.style_name);

        public bool HasFlag(int flag) => (FaceRecRaw.face_flags & flag) != 0;

        public bool HasStyle(int flag) => (FaceRecRaw.style_flags & flag) != 0;

        public uint GetCharIndexUTF32(char hi, char low)
        {
            int charCode = Char.ConvertToUtf32(hi, low);
            return FT_Get_Char_Index(m_handle, (uint)charCode); // This is almost certianly going to break.
        }

        public uint GetCharIndex(char c) => FT_Get_Char_Index(m_handle, c);

        public bool IsCharDefined(char c) => GetCharIndex(c) > 0;

        /// <summary>
        /// Gets the kerning distance between two characters.
        /// </summary>
        /// <param name="left">Character on the left</param>
        /// <param name="right">Character on the right</param>
        /// <returns>The kerning distance in pixels.</returns>
        public float GetKerning(char left, char right)
        {
            if (!HasKerning)
                return 0;

            FT_Vector aKern = new FT_Vector();

            SafeExecute(() => FT_Get_Kerning(m_handle, left, right, (int)FT_Kerning_Mode.FT_KERNING_UNFITTED, out aKern));

            return aKern.x * (1 / 64f); // Scale to pixel sizes.
        }

        /// <summary>
        /// Converts a 1-bit bitmap to a 32-bit RGBA bitmap.
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        private static void Swizzle1Bit(byte[] inData, byte[] outData)
        {
            for (int i = 0; i < outData.Length; ++i)
            {
                int byteIndex = i / 8;
                int bitIndex = 1 << (i % 8);

                // Thanks for the useless cast Microsoft.
                byte value = (byte)(((inData[byteIndex] & bitIndex) != 0) ? 0xFF : 0);

                outData[i * 4 + 0] = value;
                outData[i * 4 + 1] = value;
                outData[i * 4 + 2] = value;
                outData[i * 4 + 3] = value;
            }
        }

        /// <summary>
        /// Convert 8-bit grey scale to RGBA
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        private static void SwizzleGrey(byte[] inData, byte[] outData)
        {
            for (int i = 0; i < inData.Length; ++i)
            {
                outData[i * 4 + 0] = inData[i];
                outData[i * 4 + 1] = inData[i];
                outData[i * 4 + 2] = inData[i];
                outData[i * 4 + 3] = inData[i];
            }
        }

        /// <summary>
        /// Swizzle BGRA to RGBA
        /// </summary>
        /// <param name="c"></param>
        /// <param name="blit"></param>
        /// <exception cref="Exception"></exception>
        private static void SwizzleBGRA(byte[] inData, byte[] outData)
        {
            Array.Copy(inData, outData, inData.Length);

            for (int i = 0; i < inData.Length / 4; ++i)
            {
                // Swap B and R
                outData[i * 4 + 0] ^= outData[i * 4 + 2];
                outData[i * 4 + 2] ^= outData[i * 4 + 0];
                outData[i * 4 + 0] ^= outData[i * 4 + 2];
            }
        }

        public GlyphMetrics GetCurrentGlyphMetrics()
        {
            return new GlyphMetrics
            {
                Size = new Vector2(FaceRecRaw.glyph->metrics.width, FaceRecRaw.glyph->metrics.height),
                HorizontalBearing = new Vector2(FaceRecRaw.glyph->metrics.horiBearingX, FaceRecRaw.glyph->metrics.horiBearingY),
                VerticalBearing = new Vector2(FaceRecRaw.glyph->metrics.vertBearingX, FaceRecRaw.glyph->metrics.vertBearingY),
                VerticalAdvance = (int)FaceRecRaw.glyph->metrics.vertAdvance
            };
        }

        /// <summary>
        /// Renders a glyph using the supplied blitRow callback.
        /// </summary>
        /// <remarks>
        /// The callback should take a byte[] array which is in RGBA format and blit
        /// it to the given row of the destination bitmap.
        /// </remarks>
        /// <param name="c">Character to render</param>
        /// <param name="blitRow">Row blitting function</param>
        public GlyphMetrics RenderGlyph(char c, Bitmap bitmap)
        {
            if (Char.IsHighSurrogate(c))
                throw new Exception("Not supported yet.");

            int loadFlags = FT_LOAD_FORCE_AUTOHINT; //| FT_LOAD_RENDER;

            if (HasColor)
                loadFlags |= FT_LOAD_COLOR;

            SafeExecute(() => FT_Load_Char(m_handle, c, loadFlags));

            IntPtr glyph = (IntPtr)FaceRecRaw.glyph;

            SafeExecute(() => FT_Render_Glyph(glyph, FT_Render_Mode.FT_RENDER_MODE_NORMAL));

            FT_Pixel_Mode pixelMode = (FT_Pixel_Mode)CurrentGlyph.bitmap.pixel_mode;

            int pixelWidth = (int)CurrentGlyph.bitmap.width;
            int bitPitch = CurrentGlyph.bitmap.pitch;
            int bitSize = bitPitch * (int)CurrentGlyph.bitmap.rows;

            Span<byte> glyphBits = new Span<byte>((byte*)CurrentGlyph.bitmap.buffer, bitSize);

            bitmap.Blit(glyphBits, new Tokamak.Mathematics.Point(0, 0), pixelWidth, bitPitch);

            return GetCurrentGlyphMetrics();
        }
    }
}
