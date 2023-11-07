using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Metadata;
using System.Text;
using System.Threading.Tasks;

using Silk.NET.Vulkan;

namespace Tokamak.Vulkan.NativeWrapper
{
    internal unsafe sealed class VkRenderPass : IDisposable
    {
        private readonly VkDevice m_device;

        public VkRenderPass(VkDevice device, Format format)
        {
            m_device = device;
            Handle = CreateHandle(format);
        }

        public void Dispose()
        {
            m_device.Parent.Vk.DestroyRenderPass(m_device.LogicalDevice, Handle, null);
            GC.SuppressFinalize(this);
        }

        public RenderPass Handle { get; }

        private RenderPass CreateHandle(Format format)
        {
            var colorAttachment = new AttachmentDescription
            {
                Format = format,
                Samples = SampleCountFlags.Count1Bit,
                LoadOp = AttachmentLoadOp.Clear,
                StoreOp = AttachmentStoreOp.Store,
                StencilLoadOp = AttachmentLoadOp.DontCare,
                InitialLayout = ImageLayout.Undefined,
                FinalLayout = ImageLayout.PresentSrcKhr
            };

            var colorAttachmentRef = new AttachmentReference
            {
                Attachment = 0,
                Layout = ImageLayout.ColorAttachmentOptimal
            };

            var subpass = new SubpassDescription
            {
                PipelineBindPoint = PipelineBindPoint.Graphics,
                ColorAttachmentCount = 1,
                PColorAttachments = &colorAttachmentRef
            };

            var info = new RenderPassCreateInfo
            {
                SType = StructureType.RenderPassCreateInfo,
                AttachmentCount = 1,
                PAttachments = &colorAttachment,
                SubpassCount = 1,
                PSubpasses = &subpass
            };

            RenderPass handle = default;

            m_device.Parent.SafeExecute(vk => vk.CreateRenderPass(m_device.LogicalDevice, info, null, out handle));

            return handle;
        }
    }
}
