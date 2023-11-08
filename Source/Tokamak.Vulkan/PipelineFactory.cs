using System;
using System.IO;
using System.Linq;

using Silk.NET.Vulkan;

using Tokamak.Formats;
using Tokamak.Utils;

using Tokamak.Vulkan.NativeWrapper;

using PLHandle = Silk.NET.Vulkan.Pipeline;

namespace Tokamak.Vulkan
{
    internal unsafe class PipelineFactory : IPipelineFactory
    {
        private readonly ResourceTracker m_trackedResource = new();

        private readonly VkDevice m_device;
        private readonly PipelineConfig m_config;

        public PipelineFactory(VkDevice device, PipelineConfig config)
        {
            m_device = device;
            m_config = config;
        }

        public void Dispose()
        {
            m_trackedResource.Dispose();
        }

        private ShaderStageFlags GetFlagsFromType(ShaderType type)
        {
            return type switch
            {
                ShaderType.Fragment => ShaderStageFlags.FragmentBit,
                ShaderType.Vertex => ShaderStageFlags.VertexBit,
                ShaderType.Geometry => ShaderStageFlags.GeometryBit,
                ShaderType.Compute => ShaderStageFlags.ComputeBit,
                _ => throw new Exception($"Unknown shader type {type}")
            };
        }

        private PipelineShaderStageCreateInfo[] GetShaderModules()
        {
            var shaderList = m_config.Shaders.ToList();

            var items = new PipelineShaderStageCreateInfo[shaderList.Count];
            int index = 0;

            foreach (var shader in shaderList)
            {
                // TODO: Use VFS
                byte[] data = File.ReadAllBytes(shader.Path);
                var module = new VkShaderModule(m_device, data);

                m_trackedResource.Add(module);

                var name = new VkString("main");
                m_trackedResource.Add(name);

                items[index] = new PipelineShaderStageCreateInfo
                {
                    SType = StructureType.PipelineShaderStageCreateInfo,
                    Stage = GetFlagsFromType(shader.Type),
                    Module = module.Handle,
                    PName = name.Pointer
                };

                ++index;
            }

            return items;
        }

        private PipelineVertexInputStateCreateInfo GetVertexInputConfig()
        {
            // TODO: Read format of the vertex reflection and add here.

            return new PipelineVertexInputStateCreateInfo
            {
                SType = StructureType.PipelineVertexInputStateCreateInfo,

                VertexBindingDescriptionCount = 0,
                PVertexBindingDescriptions = null,

                VertexAttributeDescriptionCount = 0,
                PVertexAttributeDescriptions = null
            };
        }

        private PipelineRasterizationStateCreateInfo GetRasterConfig()
        {
            return new PipelineRasterizationStateCreateInfo
            {
                SType = StructureType.PipelineRasterizationStateCreateInfo,

                // Culling
                CullMode = m_config.Culling switch
                {
                    CullMode.None => CullModeFlags.None,
                    CullMode.Back => CullModeFlags.BackBit,
                    CullMode.Front => CullModeFlags.FrontBit,
                    CullMode.FrontAndBack => CullModeFlags.FrontAndBack,
                    _ => throw new Exception($"Unknown culling mode {m_config.Culling}")
                },
                FrontFace = FrontFace.Clockwise,

                // Only needed if we want a wireframe mode.
                PolygonMode = PolygonMode.Fill,
                LineWidth = 1.0f,

                // TODO: Allow this to be modified for some passes (e.g. the shadow pass)
                // NOTE: Requires a GPU feature if enabled?
                RasterizerDiscardEnable = false,

                // TODO: Allow this to be modified for some passes (e.g. the shadow pass)
                DepthBiasEnable = false,
                DepthBiasConstantFactor = 0,
                DepthBiasClamp = 0,
                DepthBiasSlopeFactor = 0
            };
        }

        public unsafe IPipeline Build()
        {
            var shaders = GetShaderModules();

            var vertexInputConfig = GetVertexInputConfig();

            var inputConfig = new PipelineInputAssemblyStateCreateInfo
            {
                SType = StructureType.PipelineInputAssemblyStateCreateInfo,
                Topology = PrimitiveTopology.TriangleList,
                PrimitiveRestartEnable = false
            };

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

            var rasterConfig = GetRasterConfig();

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

            /*
             * This weird variable shenanigan is to work around a potential memory leak if 
             * something should throw an exception while we create the full pipeline resource.
             */
            VkPipelineLayout layout = null;
            VkRenderPass renderPass = null;

            try
            {
                layout = new VkPipelineLayout(m_device);
                renderPass = new VkRenderPass(m_device, m_device.SwapChain.Format);

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
                        Layout = layout.Handle,
                        RenderPass = renderPass.Handle,
                        Subpass = 0,
                        BasePipelineIndex = -1
                    };

                    pipelineInfo.BasePipelineHandle.Handle = 0;

                    PLHandle handle = default;

                    m_device.Parent.SafeExecute(vk => vk.CreateGraphicsPipelines(m_device.LogicalDevice, default, 1, pipelineInfo, null, out handle));

                    var rval = new Pipeline(m_device, handle, layout, renderPass);

                    layout = null;
                    renderPass = null;

                    return rval;
                }
            }
            finally
            {
                layout?.Dispose();
                renderPass?.Dispose();
            }
        }
    }
}
