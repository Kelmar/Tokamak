using Silk.NET.Vulkan;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Metadata;
using System.Text;
using System.Threading.Tasks;

using Tokamak.Buffer;
using Tokamak.Vulkan.NativeWrapper;

using CBHandle = Silk.NET.Vulkan.CommandBuffer;
using PLHandle = Silk.NET.Vulkan.Pipeline;

namespace Tokamak.Vulkan
{
    internal unsafe class CommandBuffer : ICommandBuffer
    {
        private readonly VkDevice m_device;
        private readonly VkCommandPool m_pool;

        private Pipeline m_pipeline;

        public CommandBuffer(VkDevice device, VkCommandPool pool)
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

        public void BindPipeline(Pipeline pipeline)
        {
            m_pipeline = pipeline;
        }

        public void Draw(uint vertexCount, uint instanceCount, uint firstVertex, uint firstInstance)
        {
            m_device.Parent.Vk.CmdDraw(Handle, vertexCount, instanceCount, firstVertex, firstInstance);
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

        public void BeginPass()
        {
            var clearColor = new ClearValue()
            {
                Color = new() { Float32_0 = 0, Float32_1 = 0, Float32_2 = 0, Float32_3 = 1 }
            };

            var renderInfo = new RenderPassBeginInfo
            {
                SType = StructureType.RenderPassBeginInfo,
                RenderPass = m_pipeline.RenderPass.Handle,
                Framebuffer = m_pipeline.FrameBuffer.Handle,
                ClearValueCount = 1,
                PClearValues = &clearColor,
                RenderArea =
                {
                    Offset = { X = 0, Y = 0 },
                    Extent = m_device.SwapChain.Extent
                }
            };

            m_device.Parent.Vk.CmdBeginRenderPass(Handle, renderInfo, SubpassContents.Inline);

            m_device.Parent.Vk.CmdBindPipeline(Handle, PipelineBindPoint.Graphics, m_pipeline.Handle);
        }

        public void EndPass()
        {
            m_device.Parent.Vk.CmdEndRenderPass(Handle);
            m_pipeline = null;
        }

        public void ClearBoundTexture()
        {
        }

        public void ClearBuffers(GlobalBuffer buffers)
        {
        }

        public void DrawArrays(int vertexOffset, int vertexCount)
        {
            Draw((uint)vertexCount, 1, (uint)vertexOffset, 0);
        }

        public void DrawElements(int length)
        {
        }
    }
}
