using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Tokamak.Vulkan.NativeWrapper;

using PLHandle = Silk.NET.Vulkan.Pipeline;

namespace Tokamak.Vulkan
{
    internal unsafe class Pipeline : IPipeline
    {
        private readonly VkDevice m_device;

        private readonly VkPipelineLayout m_layout;
        private readonly VkRenderPass m_renderPass;

        public Pipeline(VkDevice device, PLHandle handle, VkPipelineLayout layout, VkRenderPass renderPass)
        {
            m_device = device;
            Handle = handle;
            m_layout = layout;
            m_renderPass = renderPass;
        }

        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                m_device.Parent.Vk.DestroyPipeline(m_device.LogicalDevice, Handle, null);

                m_renderPass.Dispose();
                m_layout.Dispose();
            }
        }

        public PLHandle Handle { get; }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        public void Activate()
        {
        }
    }
}
