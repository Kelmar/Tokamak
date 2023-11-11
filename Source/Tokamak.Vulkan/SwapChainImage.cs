using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Tokamak.Vulkan.NativeWrapper;

namespace Tokamak.Vulkan
{
    internal class SwapChainImage : IDisposable
    {
        public SwapChainImage(int index)
        {
            Index = index;
        }

        public void Dispose()
        {
            Fence?.Dispose();
            RenderSemaphore?.Dispose();
            ImageSemaphore?.Dispose();
            View?.Dispose();
            Image?.Dispose();
        }

        public int Index { get; }

        public VkImage Image { get; set; } = null;

        public VkImageView View { get; set; } = null;

        public VkSemaphore ImageSemaphore { get; set; } = null;

        public VkSemaphore RenderSemaphore { get; set; } = null;

        public VkFence Fence { get; set; } = null;
    }
}
