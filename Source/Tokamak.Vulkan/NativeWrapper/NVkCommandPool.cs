using System;

using Silk.NET.Vulkan;

namespace Tokamak.Vulkan.NativeWrapper
{
    internal unsafe sealed class NVkCommandPool : IDisposable
    {
        private readonly VkDevice m_device;

        public NVkCommandPool(VkDevice device)
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

            m_device.Parent.SafeExecute(vk => vk.CreateCommandPool(m_device.LogicalDevice, in createInfo, null, out handle));

            return handle;
        }
    }
}
