using System;

using Silk.NET.Vulkan;

namespace Tokamak.Vulkan.NativeWrapper
{
    internal unsafe sealed class VkPipelineLayout : IDisposable
    {
        private readonly VkDevice m_device;

        public VkPipelineLayout(VkDevice device)
        {
            m_device = device;
            Handle = CreateHandle();
        }

        public void Dispose()
        {
            m_device.Parent.Vk.DestroyPipelineLayout(m_device.LogicalDevice, Handle, null);
            GC.SuppressFinalize(this);
        }

        public PipelineLayout Handle { get; }

        private PipelineLayout CreateHandle()
        {
            // This describes the uniforms for the shaders.

            var info = new PipelineLayoutCreateInfo
            {
                SType = StructureType.PipelineLayoutCreateInfo,
                SetLayoutCount = 0,
                PushConstantRangeCount = 0
            };

            PipelineLayout handle = default;

            m_device.Parent.SafeExecute(vk => vk.CreatePipelineLayout(m_device.LogicalDevice, in info, null, out handle));

            return handle;
        }
    }
}
