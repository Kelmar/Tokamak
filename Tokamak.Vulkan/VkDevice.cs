using Silk.NET.Vulkan;

using System;
using System.Collections.Generic;
using System.Linq;

using Tokamak.Vulkan.NativeWrapper;

namespace Tokamak.Vulkan
{
    internal class VkDevice : Device, IDisposable
    {
        private readonly VkPlatform m_platform;

        private readonly List<VkQueueFamilyProperties> m_queueProps = new List<VkQueueFamilyProperties>();

        private Silk.NET.Vulkan.Device m_logicalDevice;
        private Queue m_graphicsQueue;

        private VkDevice(VkPlatform platform, VkPhysicalDevice device)
        {
            m_logicalDevice.Handle = 0;

            m_platform = platform;
            PhysicalDevice = device;
        }

        public unsafe void Dispose()
        {
            if (m_logicalDevice.Handle != 0)
            {
                m_platform.Vk.DestroyDevice(m_logicalDevice, null);
                m_logicalDevice.Handle = 0;
            }
        }

        public VkPhysicalDevice PhysicalDevice { get; }

        public static IEnumerable<VkDevice> EnumerateAll(VkPlatform platform)
        {
            var devs = VkPhysicalDevice.Enumerate(platform);

            var rval = new List<VkDevice>(devs.Count());

            foreach (var dev in devs)
            {
                var info = new VkPhysicalDeviceProperties(platform, dev);

                rval.Add(new VkDevice(platform, dev)
                {
                    VendorID = (int)info.VendorID,
                    DeviceID = (int)info.DeviceID,
                    Name = info.DeviceName,
                });
            }

            return rval;
        }

        public IEnumerable<VkQueueFamilyProperties> GetQueues()
        {
            if (!m_queueProps.Any())
                m_queueProps.AddRange(VkQueueFamilyProperties.GetQueues(m_platform, this));

            return m_queueProps;
        }

        public unsafe void InitLogicalDevice()
        {
            float queuePriority = 1.0f;

            var graphQueue = GetQueues().First(q => q.QueueFlags.HasFlag(QueueFlags.GraphicsBit));

            var queueCreateInfo = new DeviceQueueCreateInfo
            {
                SType = StructureType.DeviceQueueCreateInfo,
                QueueFamilyIndex = graphQueue.Index,
                QueueCount = 1,
                PQueuePriorities = &queuePriority,
            };

            var features = new PhysicalDeviceFeatures
            {
            };

            var createInfo = new DeviceCreateInfo
            {
                SType = StructureType.DeviceCreateInfo,
                QueueCreateInfoCount = 1,
                PQueueCreateInfos = &queueCreateInfo,
                PEnabledFeatures = &features,
                EnabledExtensionCount = 0
            };

            m_platform.SafeExecute(vk => vk.CreateDevice(PhysicalDevice.Handle, in createInfo, null, out m_logicalDevice));

            m_platform.Vk.GetDeviceQueue(m_logicalDevice, graphQueue.Index, 0, out m_graphicsQueue);
        }
    }
}
