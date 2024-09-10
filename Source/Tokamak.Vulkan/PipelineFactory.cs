using System;
using System.IO;
using System.Linq;

using Silk.NET.Vulkan;

using Tokamak.Core.Utilities;

using Tokamak.Tritium.APIs;
using Tokamak.Tritium.Pipelines;

using Tokamak.Vulkan.NativeWrapper;

namespace Tokamak.Vulkan
{
    internal unsafe class PipelineFactory : IFactory<IPipeline>
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
                _ => throw new ArgumentException($"Unknown shader type {type}")
            };
        }

        public PipelineShaderStageCreateInfo[] GetShaderModules()
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

        public PipelineVertexInputStateCreateInfo GetVertexInputConfig()
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

        public PipelineRasterizationStateCreateInfo GetRasterConfig()
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
                    _ => throw new ArgumentException($"Unknown culling mode {m_config.Culling}")
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

        public PipelineInputAssemblyStateCreateInfo GetInputConfig()
        {
            return new PipelineInputAssemblyStateCreateInfo
            {
                SType = StructureType.PipelineInputAssemblyStateCreateInfo,
                Topology = PrimitiveTopology.TriangleList,
                PrimitiveRestartEnable = false
            };
        }

        public PipelineColorBlendAttachmentState GetBlendAttachmentState()
        {
            // TODO: Pull in blending from the configuration.

            return new PipelineColorBlendAttachmentState
            {
                ColorWriteMask = ColorComponentFlags.RBit | ColorComponentFlags.GBit | ColorComponentFlags.BBit | ColorComponentFlags.ABit,
                BlendEnable = false
            };
        }

        public unsafe IPipeline Build()
        {
            return new Pipeline(m_device, this);
        }
    }
}
