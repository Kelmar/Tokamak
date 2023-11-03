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
    internal class VkDevice : Device, IDisposable
    {
        private readonly List<VkQueueFamilyProperties> m_queueProps = new List<VkQueueFamilyProperties>();

        private NativeDevice m_logicalDevice;

        private NativeQueue m_graphicsQueue;
        private NativeQueue m_surfaceQueue;

        private VkDevice(VkPlatform platform, VkPhysicalDevice device)
        {
            m_logicalDevice.Handle = 0;

            Parent = platform;
            PhysicalDevice = device;
        }

        public unsafe void Dispose()
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

        public NativeQueue SurfaceQueue => m_surfaceQueue;

        public uint SurfaceQueueIndex { get; private set; }

        public SwapChain SwapChain { get; private set; }

        public bool Initialized => LogicalDevice.Handle != 0;

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
             where T : NativeExtension<Silk.NET.Vulkan.Vk>
        {
            return Parent.Vk.TryGetDeviceExtension<T>(Parent.Instance, LogicalDevice, out ext);
        }

        internal unsafe VkImageView CreateImageView(ImageViewCreateInfo createInfo)
        {
            ImageView rval = default;

            Parent.SafeExecute(vk => vk.CreateImageView(LogicalDevice, createInfo, null, out rval));

            return VkImageView.FromHandle(rval);
        }

        public unsafe void InitLogicalDevice()
        {
            if (Initialized)
                return;

            float queuePriority = 1.0f;

            var graphQueue = GetQueues().First(q => q.QueueFlags.HasFlag(QueueFlags.GraphicsBit));
            VkQueueFamilyProperties surfaceQueue = null;

            GraphicsQueueIndex = graphQueue.Index;
            SurfaceQueueIndex = graphQueue.Index;

            var uniqueFamilies = new HashSet<uint>
            {
                graphQueue.Index
            };

            foreach (var q in GetQueues())
            {
                if (Parent.Surface.GetPhysicalDeviceSupport(PhysicalDevice, q.Index))
                {
                    SurfaceQueueIndex = q.Index;
                    uniqueFamilies.Add(q.Index);
                    surfaceQueue = q;
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

            Parent.SafeExecute(vk => vk.CreateDevice(PhysicalDevice.Handle, in createInfo, null, out m_logicalDevice));

            Parent.Vk.GetDeviceQueue(LogicalDevice, GraphicsQueueIndex, 0, out m_graphicsQueue);
            Parent.Vk.GetDeviceQueue(LogicalDevice, SurfaceQueueIndex, 0, out m_surfaceQueue);

            SwapChain = new SwapChain(this);
        }
    }
}
