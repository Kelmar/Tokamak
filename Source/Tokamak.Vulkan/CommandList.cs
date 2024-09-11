using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Reflection.Metadata;
using System.Text;
using System.Threading.Tasks;

using Silk.NET.Vulkan;

using Tokamak.Abstractions.Logging;

using Tokamak.Mathematics;

using Tokamak.Tritium.APIs;
using Tokamak.Tritium.Pipelines;

using Tokamak.Vulkan.NativeWrapper;

namespace Tokamak.Vulkan
{
    internal unsafe class CommandList : ICommandList
    {
        private readonly ILogger<CommandList> m_log;

        private readonly VkDevice m_device;
        private readonly VkCommandPool m_pool;

        private readonly VkFence m_fence;

        private SwapChainImage m_image;
        
        private Pipeline m_pipeline;
        private VkCommandBuffer m_cmdBuffer;

        private bool m_inDraw;

        public CommandList(
            ILogger<CommandList> logger,
            VkDevice device,
            VkCommandPool pool)
        {
            m_log = logger;

            m_device = device;
            m_pool = pool;

            m_fence = new VkFence(m_device, true);

            m_cmdBuffer = new VkCommandBuffer(m_device, m_pool);
        }

        public void Dispose()
        {
            m_device.WaitForSubmittedWork();

            m_fence.Wait();

            m_cmdBuffer.Dispose();
            m_fence.Dispose();
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

        private bool Reset()
        {
            if (!m_device.SwapChain.AcquireNextImage(m_fence))
            {
                //m_log.Debug("Die on reset here?");
                return false; // Probably should die here.
            }

            m_image = m_device.SwapChain.CurrentImage;

            m_cmdBuffer.Reset(CommandBufferResetFlags.None);

            return true;
        }

        public void Begin()
        {
            m_fence.Wait();
            m_fence.Reset();

            m_inDraw = Reset();

            if (!m_inDraw)
                return;

            //m_cmdBuffer.RenderArea = new Rect2D(
            //    new Offset2D(m_device.Parent.Viewport.Left, m_device.Parent.Viewport.Top),
            //    new Extent2D((uint)m_device.Parent.Viewport.Size.X, (uint)m_device.Parent.Viewport.Size.Y)
            //);

            var extent = new Point(m_device.Parent.Window.FramebufferSize.X, m_device.Parent.Window.FramebufferSize.Y);

            m_cmdBuffer.RenderArea = new Rect2D(
                new Offset2D(0, 0),
                new Extent2D((uint)extent.X, (uint)extent.Y)
            );

            m_cmdBuffer.Begin();

            m_cmdBuffer.BindPipeline(m_pipeline);

            VkFramebuffer framebuffer = m_device.SwapChain.Images[m_image.Index].Framebuffer;
            
            m_cmdBuffer.BeginRenderPass(ClearColor, m_pipeline.RenderPass, framebuffer);
        }

        public void End()
        {
            if (!m_inDraw)
                return;

            m_cmdBuffer.EndRenderPass();

            m_cmdBuffer.PipelineBarrier();

            m_cmdBuffer.End();

            m_device.QueueSubmit(m_device.GraphicsQueue, m_cmdBuffer);

            m_device.WaitForSubmittedWork();

            m_device.SwapChain.Present(m_device.PresentQueue);

            m_inDraw = false;
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
