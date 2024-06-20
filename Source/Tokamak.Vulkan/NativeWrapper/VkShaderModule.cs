using Silk.NET.Vulkan;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tokamak.Vulkan.NativeWrapper
{
    internal unsafe class VkShaderModule : IDisposable
    {
        private readonly VkDevice m_device;

        public VkShaderModule(VkDevice device, byte[] data)
        {
            m_device = device;

            Data = data;

            Handle = CreateHandle();
        }

        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                m_device.Parent.Vk.DestroyShaderModule(m_device.LogicalDevice, Handle, null);
            }
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        public byte[] Data { get; }

        public ShaderModule Handle { get; }

        private ShaderModule CreateHandle()
        {
            ShaderModule rval = default;

            var createInfo = new ShaderModuleCreateInfo
            {
                SType = StructureType.ShaderModuleCreateInfo,
                CodeSize = (nuint)Data.Length
            };

            fixed(byte* code = Data)
            {
                createInfo.PCode = (uint*)code;
                m_device.Parent.SafeExecute(vk => vk.CreateShaderModule(m_device.LogicalDevice, in createInfo, null, out rval));
            }

            return rval;
        }
    }
}
