using System;

using Tokamak.Buffer;
using Tokamak.Mathematics;

using Silk.NET.OpenGL;

using GlPixelFormat = Silk.NET.OpenGL.PixelFormat;
using TokPixelFormat = Tokamak.Formats.PixelFormat;

namespace Tokamak.OGL
{
    internal class TextureObject : ITextureObject
    {
        private readonly uint m_handle;

        private readonly GLDevice m_parent;

        private readonly GlPixelFormat m_glFormat;
        private readonly PixelType m_glType;
        private readonly InternalFormat m_glInternal;

        public TextureObject(GLDevice device, TokPixelFormat format, Point size)
        {
            m_parent = device;
            m_handle = m_parent.GL.GenTexture();

            Format = format;
            Size = new Point(MathX.NextPow2(size.X), MathX.NextPow2(size.Y));

            Bitmap = new Bitmap(Size, Format);

            m_glFormat = Format.ToGlPixelFormat();
            m_glType = Format.ToGlPixelType();
            m_glInternal = Format.ToGlInternalFormat();
        }

        public void Dispose()
        {
            m_parent.GL.DeleteTexture(m_handle);
            Bitmap.Dispose();
        }

        public TokPixelFormat Format { get; }

        public Point Size { get; }

        public Bitmap Bitmap { get; }

        public void Activate()
        {
            m_parent.GL.ActiveTexture(TextureUnit.Texture0);
            m_parent.GL.BindTexture(TextureTarget.Texture2D, m_handle);
        }

        public void Refresh()
        {
            Activate();

            m_parent.GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMinFilter, (int)TextureMinFilter.Linear);
            m_parent.GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMagFilter, (int)TextureMagFilter.Linear);

            var span = new ReadOnlySpan<byte>(Bitmap.Data);

            m_parent.GL.TexImage2D(
                TextureTarget.Texture2D,
                0,
                m_glInternal,
                (uint)Size.X,
                (uint)Size.Y,
                0,
                m_glFormat,
                m_glType,
                span);

            //GL.GenerateMipmap(GenerateMipmapTarget.Texture2D);
        }
    }
}
