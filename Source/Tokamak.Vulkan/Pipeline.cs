using System;
using System.Collections.Generic;

using Silk.NET.Vulkan;

using Tokamak.Vulkan.NativeWrapper;

using PLHandle = Silk.NET.Vulkan.Pipeline;

namespace Tokamak.Vulkan
{
    internal unsafe class Pipeline : IPipeline
    {
        private readonly VkDevice m_device;

        private readonly VkPipelineLayout m_layout;

        private readonly List<VkFramebuffer> m_frameBuffers = new List<VkFramebuffer>();

        public Pipeline(VkDevice device, PipelineFactory factory)
        {
            m_device = device;

            m_layout = new VkPipelineLayout(m_device);
            RenderPass = new VkRenderPass(m_device, m_device.SwapChain.Format);

            foreach (var image in m_device.SwapChain.Images)
                m_frameBuffers.Add(new VkFramebuffer(m_device, m_device.SwapChain.Extent, RenderPass, image.View));

            Handle = CreateHandle(factory);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                m_device.Parent.Vk.DestroyPipeline(m_device.LogicalDevice, Handle, null);

                foreach (var fb in m_frameBuffers)
                    fb.Dispose();

                m_frameBuffers.Clear();

                RenderPass.Dispose();

                m_layout.Dispose();
            }
        }
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        public VkRenderPass RenderPass { get; }

        public IReadOnlyList<VkFramebuffer> FrameBuffers => m_frameBuffers;

        public PLHandle Handle { get; }

        private PLHandle CreateHandle(PipelineFactory factory)
        {
            var shaders = factory.GetShaderModules();

            PipelineVertexInputStateCreateInfo vertexInputConfig = factory.GetVertexInputConfig();

            PipelineInputAssemblyStateCreateInfo inputConfig = factory.GetInputConfig();

            PipelineRasterizationStateCreateInfo rasterConfig = factory.GetRasterConfig();

            var viewport = new Viewport()
            {
                X = 0,
                Y = 0,
                Width = m_device.SwapChain.Extent.Width,
                Height = m_device.SwapChain.Extent.Height,
                MinDepth = 0,
                MaxDepth = 1
            };

            var scissor = new Rect2D
            {
                Offset = { X = 0, Y = 0 },
                Extent = m_device.SwapChain.Extent
            };

            var viewportConfig = new PipelineViewportStateCreateInfo
            {
                SType = StructureType.PipelineViewportStateCreateInfo,
                ViewportCount = 1,
                PViewports = &viewport,
                ScissorCount = 1,
                PScissors = &scissor
            };

            var multiSampleConfig = new PipelineMultisampleStateCreateInfo
            {
                SType = StructureType.PipelineMultisampleStateCreateInfo,
                SampleShadingEnable = false,
                RasterizationSamples = SampleCountFlags.Count1Bit
            };

            var colorBlendAttachment = new PipelineColorBlendAttachmentState
            {
                ColorWriteMask = ColorComponentFlags.RBit | ColorComponentFlags.GBit | ColorComponentFlags.BBit | ColorComponentFlags.ABit,
                BlendEnable = false
            };

            var colorBlendConfig = new PipelineColorBlendStateCreateInfo
            {
                SType = StructureType.PipelineColorBlendStateCreateInfo,
                LogicOpEnable = false,
                LogicOp = LogicOp.Copy,
                AttachmentCount = 1,
                PAttachments = &colorBlendAttachment
            };

            colorBlendConfig.BlendConstants[0] = 0;
            colorBlendConfig.BlendConstants[1] = 0;
            colorBlendConfig.BlendConstants[2] = 0;
            colorBlendConfig.BlendConstants[3] = 0;

            fixed (PipelineShaderStageCreateInfo* shaderInfo = shaders)
            {
                var pipelineInfo = new GraphicsPipelineCreateInfo
                {
                    SType = StructureType.GraphicsPipelineCreateInfo,
                    StageCount = (uint)shaders.Length,
                    PStages = shaderInfo,
                    PVertexInputState = &vertexInputConfig,
                    PInputAssemblyState = &inputConfig,
                    PViewportState = &viewportConfig,
                    PRasterizationState = &rasterConfig,
                    PMultisampleState = &multiSampleConfig,
                    PDepthStencilState = null,
                    PColorBlendState = &colorBlendConfig,
                    PDynamicState = null,
                    Layout = m_layout.Handle,
                    RenderPass = RenderPass.Handle,
                    Subpass = 0,
                    BasePipelineIndex = -1
                };

                pipelineInfo.BasePipelineHandle.Handle = 0;

                PLHandle handle = default;

                m_device.Parent.SafeExecute(vk => vk.CreateGraphicsPipelines(m_device.LogicalDevice, default, 1, pipelineInfo, null, out handle));

                return handle;
            }
        }
    }
}
