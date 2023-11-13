using System;
using System.Numerics;

using Silk.NET.Vulkan;

using CBHandle = Silk.NET.Vulkan.CommandBuffer;

namespace Tokamak.Vulkan.NativeWrapper
{
    internal unsafe class VkCommandBuffer : IDisposable
    {
        private readonly VkDevice m_device;
        private readonly VkCommandPool m_pool;

        public VkCommandBuffer(VkDevice device, VkCommandPool pool)
        {
            m_device = device;
            m_pool = pool;

            Handle = CreateHandle();
        }

        public void Dispose()
        {
            var handles = new CBHandle[] { Handle };
            m_device.Parent.Vk.FreeCommandBuffers(m_device.LogicalDevice, m_pool.Handle, handles);
        }

        public CBHandle Handle { get; }

        public Rect2D RenderArea { get; set; }

        private CBHandle CreateHandle()
        {
            var allocInfo = new CommandBufferAllocateInfo
            {
                SType = StructureType.CommandBufferAllocateInfo,
                CommandPool = m_pool.Handle,
                Level = CommandBufferLevel.Primary,
                CommandBufferCount = 1
            };

            CBHandle handle = default;

            m_device.Parent.SafeExecute(vk => vk.AllocateCommandBuffers(m_device.LogicalDevice, allocInfo, out handle));

            return handle;
        }

        public void Reset(CommandBufferResetFlags flags)
        {
            m_device.Parent.SafeExecute(vk => vk.ResetCommandBuffer(Handle, flags));
        }

        public void PipelineBarrier()
        {
            m_device.Parent.Vk.CmdPipelineBarrier(
                Handle,
                PipelineStageFlags.BottomOfPipeBit,
                PipelineStageFlags.TopOfPipeBit,
                DependencyFlags.None,
                new MemoryBarrier[0],
                new BufferMemoryBarrier[0],
                new ImageMemoryBarrier[0]
            );
        }

        public void Begin()
        {
            var info = new CommandBufferBeginInfo
            {
                SType = StructureType.CommandBufferBeginInfo
            };

            m_device.Parent.SafeExecute(vk => vk.BeginCommandBuffer(Handle, info));
        }

        public void End()
        {
            m_device.Parent.SafeExecute(vk => vk.EndCommandBuffer(Handle));
        }

        public void BeginRenderPass(in Vector4 clearColor, VkRenderPass renderPass, VkFramebuffer framebuffer)
        {
            var cc = new ClearValue()
            {
                Color = new()
                { 
                    Float32_0 = clearColor.X,
                    Float32_1 = clearColor.Y,
                    Float32_2 = clearColor.Z,
                    Float32_3 = clearColor.W
                }
            };

            var renderInfo = new RenderPassBeginInfo
            {
                SType = StructureType.RenderPassBeginInfo,
                RenderPass = renderPass.Handle,
                Framebuffer = framebuffer.Handle,
                ClearValueCount = 1,
                PClearValues = &cc,
                RenderArea = RenderArea
            };

            m_device.Parent.Vk.CmdBeginRenderPass(Handle, renderInfo, SubpassContents.Inline);
        }

        public void EndRenderPass()
        {
            m_device.Parent.Vk.CmdEndRenderPass(Handle);
        }

        public void BindPipeline(Pipeline pipeline)
        {
            m_device.Parent.Vk.CmdBindPipeline(Handle, PipelineBindPoint.Graphics, pipeline.Handle);
        }

        public void Draw(uint vertexCount, uint instanceCount, uint firstVertex, uint firstInstance)
        {
            m_device.Parent.Vk.CmdDraw(Handle, vertexCount, instanceCount, firstVertex, firstInstance);
        }
    }
}
