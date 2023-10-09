using OpenTK.Graphics.OpenGL4;

using Tokamak.Buffer;
using Tokamak.Mathematics;

using GlPixelFormat = OpenTK.Graphics.OpenGL4.PixelFormat;
using TokPixelFormat = Tokamak.Formats.PixelFormat;

namespace Tokamak.OGL
{
    internal class TextureObject : ITextureObject
    {
        private readonly int m_handle;
        private readonly int m_buffer;

        private readonly GlPixelFormat m_glFormat;
        private readonly PixelType m_glType;

        public TextureObject(TokPixelFormat format, Point size)
        {
            m_handle = GL.GenTexture();
            m_buffer = GL.GenBuffer();

            Format = format;
            Size = size;

            m_glFormat = Format.ToGlPixelFormat();
            m_glType = Format.ToGlPixelType();
        }

        public void Dispose()
        {
            GL.DeleteBuffer(m_buffer);
            GL.DeleteTexture(m_handle);
        }

        public TokPixelFormat Format { get; }

        public Point Size { get; }

        public void Activate()
        {
            GL.BindTexture(TextureTarget.Texture2D, m_handle);
        }

        public void Set(int mipLevel, byte[] data)
        {
            Activate();

            GL.BindBuffer(BufferTarget.PixelUnpackBuffer, m_buffer);
            GL.BufferData(BufferTarget.PixelUnpackBuffer, data.Length, data, BufferUsageHint.StreamDraw);

            GL.TexSubImage2D(
                TextureTarget.Texture2D,
                mipLevel,
                0,
                0,
                Size.X,
                Size.Y,
                m_glFormat,
                m_glType,
                data);

            GL.BindBuffer(BufferTarget.PixelUnpackBuffer, 0);
        }
    }
}
