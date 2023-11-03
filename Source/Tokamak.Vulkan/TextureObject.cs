using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Tokamak.Buffer;
using Tokamak.Formats;
using Tokamak.Mathematics;

using Silk.NET.Vulkan;

using TokPixelFormat = Tokamak.Formats.PixelFormat;

namespace Tokamak.Vulkan
{
    internal class TextureObject : ITextureObject
    {
        private readonly VkPlatform m_parent;

        public TextureObject(VkPlatform parent, TokPixelFormat format, Point size)
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
