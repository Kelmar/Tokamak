using System;
using System.Runtime.InteropServices;

using Silk.NET.Vulkan;

namespace Tokamak.Vulkan.NativeWrapper
{
    internal class VkPhysicalDeviceProperties
    {
        public unsafe VkPhysicalDeviceProperties(VkPlatform platform, VkPhysicalDevice device)
        {
            platform.Vk.GetPhysicalDeviceProperties(device.Handle, out PhysicalDeviceProperties info);

            ApiVersion = info.ApiVersion;
            DriverVersion = info.DriverVersion;
            VendorID = info.VendorID;
            DeviceID = info.DeviceID;

            DeviceName = Marshal.PtrToStringAnsi((IntPtr)info.DeviceName);

            DeviceType = info.DeviceType;

            var uuidSpan = new ReadOnlySpan<byte>(info.PipelineCacheUuid, 16);

            PipelineCacheUuid = new Guid(uuidSpan);
        }

        public uint ApiVersion { get; }

        public uint DriverVersion { get; }

        public uint VendorID { get; }

        public uint DeviceID { get; }

        public string DeviceName { get; }

        public PhysicalDeviceType DeviceType { get; }

        public Guid PipelineCacheUuid { get; }

        public PhysicalDeviceLimits Limits { get; }
    }
}
