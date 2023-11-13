using System;

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
            View?.Dispose();
            Image?.Dispose();
        }

        public int Index { get; }

        public VkImage Image { get; set; } = null;

        public VkImageView View { get; set; } = null;
    }
}
