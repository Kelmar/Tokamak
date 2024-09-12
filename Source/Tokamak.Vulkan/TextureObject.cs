using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Silk.NET.Vulkan;

using Tokamak.Mathematics;

using Tokamak.Tritium.Buffers;
using Tokamak.Tritium.Buffers.Formats;

using TPixelFormat = Tokamak.Tritium.Buffers.Formats.PixelFormat;

namespace Tokamak.Vulkan
{
    internal class TextureObject : ITextureObject
    {
        private readonly VkPlatform m_parent;

        public TextureObject(VkPlatform parent, TPixelFormat format, Point size)
        {
            m_parent = parent;

            Format = format;
            Size = new Point(MathX.NextPow2(size.X), MathX.NextPow2(size.Y));

            Bitmap = new Bitmap(Size, Format);
        }

        public void Dispose()
        {
        }

        public PixelFormat Format { get; }

        public Point Size { get; }

        public Bitmap Bitmap { get; }

        public void Activate()
        {
        }

        public void Refresh()
        {
        }
    }
}
