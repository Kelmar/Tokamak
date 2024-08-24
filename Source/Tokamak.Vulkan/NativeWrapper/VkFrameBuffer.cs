using System;

using Silk.NET.Vulkan;

namespace Tokamak.Vulkan.NativeWrapper
{
    internal unsafe class VkFramebuffer : IDisposable
    {
        private readonly VkDevice m_device;

        public VkFramebuffer(VkDevice device, in Extent2D extent, VkRenderPass renderPass, VkImageView view)
        {
            m_device = device;
            Extent = extent;
            Handle = CreateHandle(renderPass, view);
        }

        public void Dispose()
        {
            m_device.Parent.Vk.DestroyFramebuffer(m_device.LogicalDevice, Handle, null);
        }

        public Framebuffer Handle { get; }

        public Extent2D Extent { get; }

        private Framebuffer CreateHandle(VkRenderPass renderPass, VkImageView view)
        {
            var attachments = new ImageView[] { view.Handle };

            fixed (ImageView* views = attachments)
            {
                var createInfo = new FramebufferCreateInfo
                {
                    SType = StructureType.FramebufferCreateInfo,
                    RenderPass = renderPass.Handle,
                    AttachmentCount = 1,
                    PAttachments = views,
                    Width = Extent.Width,
                    Height = Extent.Height,
                    Layers = 1
                };

                Framebuffer handle = default;

                m_device.Parent.SafeExecute(vk => vk.CreateFramebuffer(m_device.LogicalDevice, in createInfo, null, out handle));

                return handle;
            }
        }
    }
}
