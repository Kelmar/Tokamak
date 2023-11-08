using System;

using Silk.NET.Vulkan;

using Tokamak.Vulkan.NativeWrapper;

using PLHandle = Silk.NET.Vulkan.Pipeline;

namespace Tokamak.Vulkan
{
    internal unsafe class Pipeline : IPipeline
    {
        private readonly VkDevice m_device;

        private readonly VkPipelineLayout m_layout;

        public Pipeline(VkDevice device, PLHandle handle, VkPipelineLayout layout, VkRenderPass renderPass, VkFrameBuffer frameBuffer)
        {
            m_device = device;
            Handle = handle;
            m_layout = layout;
            RenderPass = renderPass;
            FrameBuffer = frameBuffer;
        }

        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                m_device.Parent.Vk.DestroyPipeline(m_device.LogicalDevice, Handle, null);

                FrameBuffer.Dispose();
                RenderPass.Dispose();
                m_layout.Dispose();
            }
        }

        public VkRenderPass RenderPass { get; }

        public VkFrameBuffer FrameBuffer { get; }

        public PLHandle Handle { get; }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        public void Activate(ICommandBuffer buffer)
        {
            var cmdBuffer = (CommandBuffer)buffer;
            cmdBuffer.BindPipeline(this);
        }
    }
}
