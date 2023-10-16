using System.Collections.Generic;

using Silk.NET.Vulkan;

namespace Tokamak.Vulkan.NativeWrapper
{
    internal class VkQueueFamilyProperties
    {
        private unsafe VkQueueFamilyProperties(uint index, in QueueFamilyProperties queue)
        {
            Index = index;
            QueueFlags = queue.QueueFlags;
            QueueCount = queue.QueueCount;
            TimestampValidBits = queue.TimestampValidBits;
            MinImageTransferGranularity = queue.MinImageTransferGranularity;
        }

        public uint Index { get; }

        public QueueFlags QueueFlags { get; }

        public uint QueueCount { get; }

        public uint TimestampValidBits { get; }

        public Extent3D MinImageTransferGranularity { get; }

        public static unsafe IEnumerable<VkQueueFamilyProperties> GetQueues(VkPlatform platform, VkDevice device)
        {
            uint queueCount = 0;
            uint* cnt = &queueCount;

            platform.Vk.GetPhysicalDeviceQueueFamilyProperties(device.PhysicalDevice.Handle, cnt, null);

            var rval = new List<VkQueueFamilyProperties>((int)queueCount);

            if (queueCount > 0)
            {
                var queues = new QueueFamilyProperties[queueCount];

                platform.Vk.GetPhysicalDeviceQueueFamilyProperties(device.PhysicalDevice.Handle, cnt, queues);

                fixed (QueueFamilyProperties* pQueues = queues)
                {
                    for (uint i = 0; i < queueCount; ++i)
                        rval.Add(new VkQueueFamilyProperties(i, pQueues[i]));
                }
            }

            return rval;
        }
    }
}
