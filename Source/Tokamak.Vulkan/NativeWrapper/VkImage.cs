using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Silk.NET.Vulkan;

namespace Tokamak.Vulkan.NativeWrapper
{
    internal sealed class VkImage : IDisposable
    {
        private Image m_handle;

        private VkImage(Image handle, Format format, in Extent2D extent)
        {
            m_handle = handle;
            ImageFormat = format;
            Extent = extent;
        }

        public void Dispose()
        {
            // May need to use some kind of lifetime manager, looks like the SwapChain handles the destruction of these for us.
        }

        public Format ImageFormat { get; }

        public Extent2D Extent { get; }

        public static VkImage FromHandle(Image image, Format format, in Extent2D extent)
        {
            return new VkImage(image, format, extent);
        }
    }
}
