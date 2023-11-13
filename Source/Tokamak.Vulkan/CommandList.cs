using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Reflection.Metadata;
using System.Text;
using System.Threading.Tasks;

using Tokamak.Buffer;
using Tokamak.Vulkan.NativeWrapper;

using Silk.NET.Vulkan;

namespace Tokamak.Vulkan
{
    internal unsafe class CommandList : ICommandList
    {
        private readonly VkDevice m_device;
        private readonly VkCommandPool m_pool;

        private readonly VkFence m_fence;

        private SwapChainImage m_image;
        
        private Pipeline m_pipeline;
        private VkCommandBuffer m_cmdBuffer;

        public CommandList(VkDevice device, VkCommandPool pool)
        {
            m_device = device;
            m_pool = pool;

            m_fence = new VkFence(m_device, true);

            m_cmdBuffer = new VkCommandBuffer(m_device, m_pool);
        }

        public void Dispose()
        {
        }

        public IPipeline Pipeline
        {
            get => m_pipeline;
            set => m_pipeline = (Pipeline)value;
        }

        public Vector4 ClearColor { get; set; }

        public void Draw(uint vertexCount, uint instanceCount, uint firstVertex, uint firstInstance)
        {
            m_cmdBuffer.Draw(vertexCount, instanceCount, firstVertex, firstInstance);
        }

        private void Reset()
        {
            if (!m_device.SwapChain.AcquireNextImage(m_fence))
                return; // Probably should die here.

            m_image = m_device.SwapChain.CurrentImage;

            m_cmdBuffer.Reset(CommandBufferResetFlags.None);
        }

        public void Begin()
        {
            m_fence.Wait();
            m_fence.Reset();

            Reset();

            m_cmdBuffer.RenderArea = new Rect2D(
                new Offset2D(m_device.Parent.Viewport.Left, m_device.Parent.Viewport.Top),
                new Extent2D((uint)m_device.Parent.Viewport.Size.X, (uint)m_device.Parent.Viewport.Size.Y)
            );

            m_cmdBuffer.Begin();

            m_cmdBuffer.BindPipeline(m_pipeline);

            m_cmdBuffer.BeginRenderPass(ClearColor, m_pipeline.RenderPass, m_pipeline.FrameBuffers[m_image.Index]);
        }

        public void End()
        {
            m_cmdBuffer.EndRenderPass();

            m_cmdBuffer.PipelineBarrier();

            m_cmdBuffer.End();

            m_device.QueueSubmit(m_device.GraphicsQueue, m_cmdBuffer);

            m_device.WaitForSubmittedWork();

            m_device.SwapChain.Present(m_device.PresentQueue);
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
