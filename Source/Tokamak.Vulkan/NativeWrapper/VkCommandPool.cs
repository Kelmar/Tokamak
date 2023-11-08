using System;

using Silk.NET.Vulkan;

namespace Tokamak.Vulkan.NativeWrapper
{
    internal unsafe sealed class VkCommandPool : IDisposable
    {
        private readonly VkDevice m_device;

        public VkCommandPool(VkDevice device)
        {
            m_device = device;
            Handle = CreateHandle();
        }

        public void Dispose()
        {
            m_device.Parent.Vk.DestroyCommandPool(m_device.LogicalDevice, Handle, null);
        }

        public CommandPool Handle { get; }

        private CommandPool CreateHandle()
        {
            var createInfo = new CommandPoolCreateInfo
            {
                SType = StructureType.CommandPoolCreateInfo,
                Flags = CommandPoolCreateFlags.ResetCommandBufferBit,
                QueueFamilyIndex = m_device.GraphicsQueueIndex
            };

            CommandPool handle = default;

            m_device.Parent.SafeExecute(vk => vk.CreateCommandPool(m_device.LogicalDevice, createInfo, null, out handle));

            return handle;
        }

        public VkCommandBuffer AllocateBuffer()
        {
            return new VkCommandBuffer(m_device, this);
        }
    }
}
