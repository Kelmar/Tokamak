using System;

using Tokamak.Mathematics;

using Tokamak.Tritium.Buffers;

using Silk.NET.OpenGL;

using GlPixelFormat = Silk.NET.OpenGL.PixelFormat;
using TPixelFormat = Tokamak.Tritium.Buffers.Formats.PixelFormat;

namespace Tokamak.OGL
{
    internal class TextureObject : ITextureObject
    {
        private readonly uint m_handle;

        private readonly OpenGLLayer m_layer;

        private readonly GlPixelFormat m_glFormat;
        private readonly PixelType m_glType;
        private readonly InternalFormat m_glInternal;

        public TextureObject(OpenGLLayer layer, TPixelFormat format, Point size)
        {
            m_layer = layer;
            m_handle = m_layer.GL.GenTexture();

            Format = format;
            Size = new Point(MathX.NextPow2(size.X), MathX.NextPow2(size.Y));

            Bitmap = new Bitmap(Size, Format);

            m_glFormat = Format.ToGlPixelFormat();
            m_glType = Format.ToGlPixelType();
            m_glInternal = Format.ToGlInternalFormat();
        }

        public void Dispose()
        {
            m_layer.GL.DeleteTexture(m_handle);
            Bitmap.Dispose();
        }

        public TPixelFormat Format { get; }

        public Point Size { get; }

        public Bitmap Bitmap { get; }

        public void Activate()
        {
            m_layer.GL.ActiveTexture(TextureUnit.Texture0);
            m_layer.GL.BindTexture(TextureTarget.Texture2D, m_handle);
        }

        public void Refresh()
        {
            Activate();

            m_layer.GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMinFilter, (int)TextureMinFilter.Linear);
            m_layer.GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMagFilter, (int)TextureMagFilter.Linear);

            var span = new ReadOnlySpan<byte>(Bitmap.Data);

            m_layer.GL.TexImage2D(
                TextureTarget.Texture2D,
                0,
                m_glInternal,
                (uint)Size.X,
                (uint)Size.Y,
                0,
                m_glFormat,
                m_glType,
                span);

            //m_layer.GL.GenerateMipmap(GenerateMipmapTarget.Texture2D);
        }
    }
}
