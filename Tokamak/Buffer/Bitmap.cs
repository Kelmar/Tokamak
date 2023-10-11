using System;
using System.Diagnostics;

using Tokamak.Formats;
using Tokamak.Mathematics;

namespace Tokamak.Buffer
{
    public class Bitmap
    {
        private readonly int m_pixelSize;

        public Bitmap(in Point size, PixelFormat format)
        {
            //Size = new Point(MathX.NextPow2(size.X), MathX.NextPow2(size.Y));
            Size = size;
            Format = format;

            m_pixelSize = Format switch
            {
                PixelFormat.FormatA8 => 1,
                PixelFormat.FormatR8G8B8 => 3,
                PixelFormat.FormatR8G8B8A8 => 4,
                _ => throw new Exception($"Unsupported bitmap format {Format}")
            };

            Pitch = MathX.NextPow2(Size.X) * m_pixelSize;
            int len = Pitch * MathX.NextPow2(Size.Y);
            Data = new byte[len];
        }

        public Point Size { get; }

        public int Pitch { get; }

        public PixelFormat Format { get; }

        public byte[] Data { get; }

        public void Blit(in Span<byte> data, in Point loc, int width, int pitch)
        {
            Debug.Assert(width > 0 && pitch > 0, "Invalid width/pitch");
            Debug.Assert(width <= pitch, "Invalid width/pitch");

            if (width + loc.X > Size.X)
                width = Size.X - loc.X;

            int height = data.Length / pitch;
            int copySize = width * m_pixelSize;

            if (height + loc.Y > Size.Y)
                height = Size.Y - loc.Y;

            int outOffset = (loc.Y * Size.X + loc.X) * m_pixelSize;
            int inOffset = 0;

            for (int y = 0; y < height; ++y)
            {
                Span<byte> inData = data.Slice(inOffset, pitch);
                Span<byte> outData = new Span<byte>(Data, outOffset, copySize);

                inData.CopyTo(outData);

                outOffset += Pitch;
                inOffset += pitch;
            }
        }

        public void Blit(Bitmap source, in Point loc)
        {
            int width = source.Size.X;

            if (width + loc.X > Size.X)
                width = Size.X - loc.X;

            int height = source.Size.Y;
            int copySize = width * m_pixelSize;

            if (height + loc.Y > Size.Y)
                height = Size.Y - loc.Y;

            int inOffset = 0;
            int outOffset = (loc.Y * Size.X + loc.X) * m_pixelSize;

            for (int y = 0; y < height; ++y)
            {
                Array.Copy(source.Data, inOffset, Data, outOffset, copySize);
                inOffset += source.Pitch;
            }
        }
    }
}
