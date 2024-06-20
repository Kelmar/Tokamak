using System;

using Silk.NET.Vulkan;

namespace Tokamak.Vulkan.NativeWrapper
{
    internal unsafe sealed class VkSemaphore : IDisposable
    {
        private readonly VkDevice m_device;

        public VkSemaphore(VkDevice device)
        {
            m_device = device;
            Handle = CreateHandle();
        }

        public void Dispose()
        {
            m_device.Parent.Vk.DestroySemaphore(m_device.LogicalDevice, Handle, null);
        }

        public Semaphore Handle { get; }

        private Semaphore CreateHandle()
        {
            var createInfo = new SemaphoreCreateInfo
            {
                SType = StructureType.SemaphoreCreateInfo,
                Flags = SemaphoreCreateFlags.None
            };

            Semaphore handle = default;

            m_device.Parent.SafeExecute(vk => vk.CreateSemaphore(m_device.LogicalDevice, in createInfo, null, out handle));

            return handle;
        }
    }
}
