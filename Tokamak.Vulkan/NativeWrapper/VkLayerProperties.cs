using Silk.NET.Vulkan;

using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;

namespace Tokamak.Vulkan.NativeWrapper
{
    internal class VkLayerProperties
    {
        private VkLayerProperties()
        {
        }

        public string LayerName { get; set; }

        public uint SpecVersion { get; set; }

        public uint ImplementationVersion { get; set; }

        public string Description { get; set; }

        public static unsafe IEnumerable<VkLayerProperties> InstanceEnumerate(VkPlatform platform)
        {
            uint layerCnt = 0;
            uint* cnt = &layerCnt;

            platform.SafeExecute(vk => vk.EnumerateInstanceLayerProperties(cnt, null));

            var rval = new List<VkLayerProperties>((int)layerCnt);

            if (layerCnt > 0)
            {
                var layers = new LayerProperties[layerCnt];

                platform.SafeExecute(vk => vk.EnumerateInstanceLayerProperties(cnt, layers));

                fixed (LayerProperties* pLayer = layers)
                {
                    for (uint i = 0; i < layerCnt; ++i)
                    {
                        string name = Marshal.PtrToStringAnsi((IntPtr)pLayer[i].LayerName);
                        string desc = Marshal.PtrToStringAnsi((IntPtr)pLayer[i].Description);

                        rval.Add(new VkLayerProperties
                        {
                            LayerName = name,
                            Description = desc,
                            SpecVersion = pLayer[i].SpecVersion,
                            ImplementationVersion = pLayer[i].ImplementationVersion
                        });
                    }
                }
            }

            return rval;
        }
    }
}
