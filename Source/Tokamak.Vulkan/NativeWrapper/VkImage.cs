using System;

using Silk.NET.Vulkan;

namespace Tokamak.Vulkan.NativeWrapper
{
    internal sealed class VkImage : IDisposable
    {
        private VkImage(VkDevice device, Image handle, Format format, in Extent3D extent)
        {
            Device = device;
            Handle = handle;
            ImageFormat = format;
            Extent = extent;
        }

        public void Dispose()
        {
            // May need to use some kind of lifetime manager, looks like the SwapChain handles the destruction of these for us.
        }

        public VkDevice Device { get; }

        public Image Handle { get; }

        public Format ImageFormat { get; }

        public Extent3D Extent { get; }

        public unsafe VkImageView CreateView()
        {
            var createInfo = new ImageViewCreateInfo
            {
                SType = StructureType.ImageViewCreateInfo,
                PNext = null,
                Image = Handle,
                ViewType = ImageViewType.Type2D,
                Format = ImageFormat,
                Components =
                {
                    R = ComponentSwizzle.Identity,
                    G = ComponentSwizzle.Identity,
                    B = ComponentSwizzle.Identity,
                    A = ComponentSwizzle.Identity
                },
                SubresourceRange =
                {
                    AspectMask = ImageAspectFlags.ColorBit,
                    BaseMipLevel = 0,
                    LevelCount = 1,
                    BaseArrayLayer = 0,
                    LayerCount = 1
                }
            };

            ImageView view = default;

            Device.Parent.SafeExecute(vk => vk.CreateImageView(Device.LogicalDevice, ref createInfo, null, out view));

            return VkImageView.FromHandle(Device, view);
        }

        public static VkImage FromHandle(VkDevice device, Image image, Format format, in Extent3D extent)
        {
            return new VkImage(device, image, format, extent);
        }
    }
}
