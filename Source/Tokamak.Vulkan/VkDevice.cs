using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;

using Silk.NET.Core.Native;
using Silk.NET.Vulkan;
using Silk.NET.Vulkan.Extensions.KHR;

using Tokamak.Config;
using Tokamak.Vulkan.NativeWrapper;

using NativeDevice = Silk.NET.Vulkan.Device;
using NativeQueue = Silk.NET.Vulkan.Queue;

namespace Tokamak.Vulkan
{
    internal unsafe class VkDevice : Device, IDisposable
    {
        private readonly List<VkQueueFamilyProperties> m_queueProps = new List<VkQueueFamilyProperties>();

        private NativeDevice m_logicalDevice;

        private NativeQueue m_graphicsQueue;
        private NativeQueue m_presentQueue;

        private VkDevice(VkPlatform platform, VkPhysicalDevice device)
        {
            m_logicalDevice.Handle = 0;

            Parent = platform;
            PhysicalDevice = device;
        }

        public void Dispose()
        {
            SwapChain?.Dispose();

            if (LogicalDevice.Handle != 0)
            {
                Parent.Vk.DestroyDevice(LogicalDevice, null);
                m_logicalDevice.Handle = 0;
            }
        }

        public VkPlatform Parent { get; }

        public VkPhysicalDevice PhysicalDevice { get; }

        public NativeDevice LogicalDevice => m_logicalDevice;

        public NativeQueue GraphicsQueue => m_graphicsQueue;

        public uint GraphicsQueueIndex { get; private set; }

        public NativeQueue PresentQueue => m_presentQueue;

        public uint PresentQueueIndex { get; private set; }

        public SwapChain SwapChain { get; private set; }

        public bool Initialized => LogicalDevice.Handle != 0;

        public static IEnumerable<VkDevice> EnumerateAll(VkPlatform platform)
        {
            var devices = VkPhysicalDevice.Enumerate(platform);

            var rval = new List<VkDevice>(devices.Count());

            foreach (var dev in devices)
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

        public void WaitIdle()
        {
            Parent.SafeExecute(vk => vk.DeviceWaitIdle(LogicalDevice));
        }

        public IEnumerable<VkQueueFamilyProperties> GetQueues()
        {
            if (!m_queueProps.Any())
                m_queueProps.AddRange(VkQueueFamilyProperties.GetQueues(Parent, this));

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
            yield return KhrSwapchain.ExtensionName;
        }

        public bool TryGetExtension<T>(out T ext)
             where T : NativeExtension<Vk>
        {
            return Parent.Vk.TryGetDeviceExtension(Parent.Instance, LogicalDevice, out ext);
        }

        internal VkImage CreateImage(ImageCreateInfo createInfo)
        {
            Image image = default;

            Parent.SafeExecute(vk => vk.CreateImage(LogicalDevice, createInfo, null, out image));

            return VkImage.FromHandle(this, image, createInfo.Format, createInfo.Extent);
        }

        public void InitLogicalDevice()
        {
            if (Initialized)
                return;

            float queuePriority = 1.0f;

            var queues = GetQueues().ToList();

            var graphQueue = queues.First(q => q.QueueFlags.HasFlag(QueueFlags.GraphicsBit));
            VkQueueFamilyProperties presentQueue = null;

            GraphicsQueueIndex = graphQueue.Index;
            PresentQueueIndex = graphQueue.Index;

            var uniqueFamilies = new HashSet<uint>
            {
                graphQueue.Index
            };

            foreach (var q in GetQueues())
            {
                if (Parent.Surface.GetPhysicalDeviceSupport(PhysicalDevice, q.Index))
                {
                    PresentQueueIndex = q.Index;
                    uniqueFamilies.Add(q.Index);
                    presentQueue = q;
                    break;
                }
            }

            var ufArray = uniqueFamilies.ToArray();

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
            using var extensions = new VkStringArray(GetEnabledExtensions());

            var createInfo = new DeviceCreateInfo
            {
                SType = StructureType.DeviceCreateInfo,
                QueueCreateInfoCount = (uint)ufArray.Length,
                PQueueCreateInfos = queueCreateInfos,
                PEnabledFeatures = &features,
                PpEnabledExtensionNames = extensions.Pointer,
                EnabledExtensionCount = extensions.Length,
                PpEnabledLayerNames = layers.Pointer,
                EnabledLayerCount = layers.Length
            };

            Parent.SafeExecute(vk => vk.CreateDevice(PhysicalDevice.Handle, in createInfo, null, out m_logicalDevice));

            Parent.Vk.GetDeviceQueue(LogicalDevice, GraphicsQueueIndex, 0, out m_graphicsQueue);
            Parent.Vk.GetDeviceQueue(LogicalDevice, PresentQueueIndex, 0, out m_presentQueue);

            SwapChain = new SwapChain(this);
        }
    }
}
