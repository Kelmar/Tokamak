﻿using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;

using Silk.NET.Vulkan;

namespace Tokamak.Vulkan.NativeWrapper
{
    internal class VkExtensionsProperties
    {

        private VkExtensionsProperties(string extensionName, uint specVersion)
        {
            ExtensionName = extensionName;
            SpecVersion = specVersion;
        }

        public string ExtensionName { get; }

        public uint SpecVersion { get; }

        public static unsafe IEnumerable<VkExtensionsProperties> InstanceEnumerate(VkPlatform platform)
        {
            uint extCnt = 0;
            uint* cnt = &extCnt;

            platform.SafeExecute(vk => vk.EnumerateInstanceExtensionProperties((byte*)null, cnt, null));

            var rval = new List<VkExtensionsProperties>((int)extCnt);

            if (extCnt > 0)
            {
                var exts = new ExtensionProperties[extCnt];

                platform.SafeExecute(vk => vk.EnumerateInstanceExtensionProperties((byte*)null, cnt, exts));

                fixed (ExtensionProperties* pExt = exts)
                {
                    for (uint i = 0; i < extCnt; ++i)
                    {
                        string name = Marshal.PtrToStringAnsi((IntPtr)pExt[i].ExtensionName);

                        rval.Add(new VkExtensionsProperties(name, pExt[i].SpecVersion));
                    }
                }
            }

            return rval;
        }
    }
}
