using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Silk.NET.Vulkan;

namespace Tokamak.Vulkan.NativeWrapper
{
    internal class VkImageView
    {
        private readonly ImageView m_imageView;

        public VkImageView(Image image, Format format)
        {
            m_imageView = CreateObject(image, format);
        }

        private ImageView CreateObject(Image image, Format format)
        {
            var createInfo = new ImageViewCreateInfo
            {
                SType = StructureType.ImageViewCreateInfo,
                PNext = null,
                Image = image,
                ViewType = ImageViewType.Type2D,
                Format = format,
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

            ImageView rval = default;

            return rval;
        }
    }
}
