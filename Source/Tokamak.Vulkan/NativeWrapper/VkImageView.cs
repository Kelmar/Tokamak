using System;

using Silk.NET.Vulkan;

namespace Tokamak.Vulkan.NativeWrapper
{
    internal sealed class VkImageView : IDisposable
    {
        private VkImageView(VkDevice device, ImageView handle)
        {
            Device = device;
            Handle = handle;
        }

        public unsafe void Dispose()
        {
            Device.Parent.Vk.DestroyImageView(Device.LogicalDevice, Handle, null);
        }

        public VkDevice Device { get; }

        public ImageView Handle { get; }

        public static VkImageView FromHandle(VkDevice device, ImageView handle)
        {
            return new VkImageView(device, handle);
        }
    }
}
