using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;

using Silk.NET.Core.Native;
using Silk.NET.Vulkan;

using Tokamak.Config;
using Tokamak.Vulkan.NativeWrapper;

using NativeDevice = Silk.NET.Vulkan.Device;
using NativeQueue = Silk.NET.Vulkan.Queue;

namespace Tokamak.Vulkan
{
    internal class VkDevice : Device, IDisposable
    {
        private readonly VkPlatform m_platform;

        private readonly List<VkQueueFamilyProperties> m_queueProps = new List<VkQueueFamilyProperties>();

        private NativeDevice m_logicalDevice;

        private NativeQueue m_graphicsQueue;
        private NativeQueue m_surfaceQueue;

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

        public bool Initialized => m_logicalDevice.Handle != 0;

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

        private IEnumerable<string> GetEnabledLayers()
        {
            var config = Platform.Services.Find<IConfigReader>();

            if (config.Get(VkPlatform.VK_VALIDATE_CALLS_CONFIG, false))
                yield return VkPlatform.VK_VALIDATE_LAYER_NAME;
        }

        private IEnumerable<string> GetEnabledExtensions()
        {
            yield break;
        }

        public unsafe void InitLogicalDevice()
        {
            if (Initialized)
                return;

            float queuePriority = 1.0f;

            var graphQueue = GetQueues().First(q => q.QueueFlags.HasFlag(QueueFlags.GraphicsBit));
            VkQueueFamilyProperties surfaceQueue = null;

            var uniqueFamilys = new HashSet<uint>();
            uniqueFamilys.Add(graphQueue.Index);

            foreach (var q in GetQueues())
            {
                if (m_platform.Surface.GetPhysicalDeviceSurfaceSupport(PhysicalDevice, q.Index))
                {
                    uniqueFamilys.Add(q.Index);
                    surfaceQueue = q;
                    break;
                }
            }

            var ufArray = uniqueFamilys.ToArray();

            using var memory = GlobalMemory.Allocate(ufArray.Length * sizeof(DeviceQueueCreateInfo));
            var queueCreateInfos = (DeviceQueueCreateInfo*)Unsafe.AsPointer(ref memory.GetPinnableReference());

            for (uint i = 0; i < ufArray.Length; ++i)
            {
                queueCreateInfos[i] = new DeviceQueueCreateInfo
                {
                    SType = StructureType.DeviceQueueCreateInfo,
                    QueueFamilyIndex = ufArray[i],
                    QueueCount = 1,
                    PQueuePriorities = &queuePriority,
                };
            }

            var features = new PhysicalDeviceFeatures
            {
            };

            using var layers = new VkStringArray(GetEnabledLayers());
            using var exts = new VkStringArray(GetEnabledExtensions());

            var createInfo = new DeviceCreateInfo
            {
                SType = StructureType.DeviceCreateInfo,
                QueueCreateInfoCount = (uint)ufArray.Length,
                PQueueCreateInfos = queueCreateInfos,
                PEnabledFeatures = &features,
                PpEnabledExtensionNames = exts.Pointer,
                EnabledExtensionCount = exts.Length,
                PpEnabledLayerNames = layers.Pointer,
                EnabledLayerCount = layers.Length
            };

            m_platform.SafeExecute(vk => vk.CreateDevice(PhysicalDevice.Handle, in createInfo, null, out m_logicalDevice));

            m_platform.Vk.GetDeviceQueue(m_logicalDevice, graphQueue.Index, 0, out m_graphicsQueue);

            if (surfaceQueue != null)
                m_platform.Vk.GetDeviceQueue(m_logicalDevice, surfaceQueue.Index, 0, out m_surfaceQueue);
        }
    }
}
