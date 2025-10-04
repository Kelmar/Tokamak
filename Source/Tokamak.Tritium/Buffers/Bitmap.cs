using System;
using System.Buffers;
using System.Diagnostics;

using Tokamak.Mathematics;

using Tokamak.Tritium.Buffers.Formats;

namespace Tokamak.Tritium.Buffers
{
    public class Bitmap : IDisposable
    {
        private readonly int m_bytesPerPixel;

        public Bitmap(in Point size, PixelFormat format)
        {
            Size = size;
            Format = format;

            m_bytesPerPixel = Format switch
            {
                PixelFormat.FormatA8 => 1,
                PixelFormat.FormatR8G8B8 => 3,
                PixelFormat.FormatR8G8B8A8 => 4,
                _ => throw new Exception($"Unsupported bitmap format {Format}")
            };

            Pitch = MathX.NextPow2(Size.X) * m_bytesPerPixel;
            int len = Pitch * MathX.NextPow2(Size.Y);

            Data = ArrayPool<byte>.Shared.Rent(len);
        }

        public void Dispose()
        {
            ArrayPool<byte>.Shared.Return(Data);
        }

        public bool Dirty { get; private set; }

        public Point Size { get; }

        public int Pitch { get; }

        public PixelFormat Format { get; }

        public byte[] Data { get; }

        public void Clear()
        {
            Array.Fill<byte>(Data, 0);
        }

        public void NotDirty()
        {
            Dirty = false;
        }

        public void Blit(in Span<byte> data, in Point loc, int width, int pitch)
        {
            Debug.Assert(width > 0 && pitch > 0, "Invalid width/pitch");
            Debug.Assert(width <= pitch, "Invalid width/pitch");

            if (width + loc.X > Size.X)
                width = Size.X - loc.X;

            int height = data.Length / pitch;
            int copySize = width * m_bytesPerPixel;

            if (height + loc.Y > Size.Y)
                height = Size.Y - loc.Y;

            int outOffset = (loc.Y * Size.X + loc.X) * m_bytesPerPixel;
            int inOffset = 0;

            for (int y = 0; y < height; ++y)
            {
                Span<byte> inData = data.Slice(inOffset, copySize);
                Span<byte> outData = new Span<byte>(Data, outOffset, copySize);

                inData.CopyTo(outData);

                outOffset += Pitch;
                inOffset += pitch;
            }

            Dirty = true;
        }

        public void Blit(Bitmap source, in Point loc)
        {
            int width = source.Size.X;

            if (width + loc.X > Size.X)
                width = Size.X - loc.X;

            int height = source.Size.Y;
            int copySize = width * m_bytesPerPixel;

            if (height + loc.Y > Size.Y)
                height = Size.Y - loc.Y;

            int inOffset = 0;
            int outOffset = (loc.Y * Size.X + loc.X) * m_bytesPerPixel;

            for (int y = 0; y < height; ++y)
            {
                Array.Copy(source.Data, inOffset, Data, outOffset, copySize);
                inOffset += source.Pitch;
            }

            Dirty = true;
        }
    }
}
