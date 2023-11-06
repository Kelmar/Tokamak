using System;
using System.IO;
using System.Linq;

using Silk.NET.Vulkan;

using Tokamak.Formats;
using Tokamak.Utils;
using Tokamak.Vulkan.NativeWrapper;

namespace Tokamak.Vulkan
{
    internal unsafe class PipelineFactory
    {
        private readonly ResourceTracker m_trackedResource = new();

        private readonly VkDevice m_device;
        private readonly PipelineConfig m_config;

        private VectorFormat.Info m_format = null;

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

        public void SetInputFormat<T>()
            where T : struct
        {
            m_format = VectorFormat.GetLayoutOf<T>();
        }

        private void ValidateSettings()
        {
            if (m_format == null)
                throw new InvalidOperationException("Vector data format has not been specified.  Call SetInputFormat<T>()");
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

        private PipelineVertexInputStateCreateInfo GetVertexConfig()
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

        public IPipeline Build()
        {
            ValidateSettings();

            var items = GetShaderModules();

            var vertexConfig = GetVertexConfig();

            var rasterConfig = GetRasterConfig();

            // TODO: Create the render passes here.

            // TODO: Create the pipeline here.

            return null;
        }
    }
}
