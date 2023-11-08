using System;

using Silk.NET.Vulkan;

using CBHandle = Silk.NET.Vulkan.CommandBuffer;

namespace Tokamak.Vulkan.NativeWrapper
{
    internal unsafe sealed class VkCommandBuffer : IDisposable
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
    }
}
