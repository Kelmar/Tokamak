using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;

using Silk.NET.Vulkan;


namespace Tokamak.Vulkan.NativeWrapper
{
    internal class VkPhysicalDevice
    {
        private VkPhysicalDevice(PhysicalDevice handle)
        {
            Handle = handle;
        }

        public PhysicalDevice Handle { get; }

        public static unsafe IEnumerable<VkPhysicalDevice> Enumerate(VkPlatform platform)
        {
            uint devCnt = 0;
            uint* cnt = &devCnt;

            platform.SafeExecute(vk => vk.EnumeratePhysicalDevices(platform.Instance, cnt, null));

            var rval = new List<VkPhysicalDevice>((int)devCnt);

            if (devCnt != 0)
            {
                var devs = new PhysicalDevice[devCnt];

                platform.SafeExecute(vk => vk.EnumeratePhysicalDevices(platform.Instance, cnt, devs));

                for (int i = 0; i < devCnt; ++i)
                    rval.Add(new VkPhysicalDevice(devs[i]));
            }

            return rval;
        }
    }
}
