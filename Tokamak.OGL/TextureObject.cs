using OpenTK.Graphics.OpenGL4;

using System;

using Tokamak.Buffer;
using Tokamak.Mathematics;

using GlPixelFormat = OpenTK.Graphics.OpenGL4.PixelFormat;
using TokPixelFormat = Tokamak.Formats.PixelFormat;

namespace Tokamak.OGL
{
    internal class TextureObject : ITextureObject
    {
        private readonly int m_handle;

        private readonly GlPixelFormat m_glFormat;
        private readonly PixelType m_glType;
        private readonly PixelInternalFormat m_glInternal;

        public TextureObject(TokPixelFormat format, Point size)
        {
            m_handle = GL.GenTexture();

            Format = format;
            Size = new Point(MathX.NextPow2(size.X), MathX.NextPow2(size.Y));

            m_glFormat = Format.ToGlPixelFormat();
            m_glType = Format.ToGlPixelType();
            m_glInternal = Format.ToGlInternalFormat();
        }

        public void Dispose()
        {
            GL.DeleteTexture(m_handle);
        }

        public TokPixelFormat Format { get; }

        public Point Size { get; }

        public void Activate()
        {
            GL.ActiveTexture(TextureUnit.Texture0);
            GL.BindTexture(TextureTarget.Texture2D, m_handle);
        }

        public void Set(Bitmap bitmap)
        {
            Activate();

            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMinFilter, (int)TextureMinFilter.Linear);
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMagFilter, (int)TextureMagFilter.Linear);

            byte[] damnIt = new byte[bitmap.Data.Length];
            Array.Copy(bitmap.Data, damnIt, bitmap.Data.Length);

            // This causes a hard crash if you don't get the format right.
            GL.TexImage2D(
                TextureTarget.Texture2D,
                0,
                m_glInternal,
                Size.X,
                Size.Y,
                0,
                m_glFormat,
                m_glType,
                bitmap.Data);

            //GL.GenerateMipmap(GenerateMipmapTarget.Texture2D);
        }
    }
}
