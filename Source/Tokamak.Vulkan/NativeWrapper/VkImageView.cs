using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Microsoft.DotNet.PlatformAbstractions;
using Silk.NET.Vulkan;

using static System.Net.Mime.MediaTypeNames;

namespace Tokamak.Vulkan.NativeWrapper
{
    internal class VkImageView : IDisposable
    {
        private readonly ImageView m_handle;

        private VkImageView(ImageView handle)
        {
            m_handle = handle;
        }

        public void Dispose()
        {
            // May need to use some kind of lifetime manager, looks like the SwapChain handles the destruction of these for us.
        }

        public static VkImageView FromHandle(ImageView handle)
        {
            return new VkImageView(handle);
        }

        public static VkImageView FromDevice(VkDevice device, VkImage image)
        {
            var createInfo = new ImageViewCreateInfo
            {
                SType = StructureType.ImageViewCreateInfo,
                PNext = null,
                Image = image.Handle,
                ViewType = ImageViewType.Type2D,
                Format = image.ImageFormat,
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

            return device.CreateImageView(createInfo);
        }
    }
}
