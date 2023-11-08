using Silk.NET.Vulkan;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Metadata;
using System.Text;
using System.Threading.Tasks;

using Tokamak.Buffer;
using Tokamak.Vulkan.NativeWrapper;

using CBHandle = Silk.NET.Vulkan.CommandBuffer;
using PLHandle = Silk.NET.Vulkan.Pipeline;

namespace Tokamak.Vulkan
{
    internal class CommandBuffer : ICommandBuffer
    {
        private readonly VkDevice m_device;
        private readonly VkCommandPool m_pool;

        public CommandBuffer(VkDevice device, VkCommandPool pool)
        {
            m_device = device;
            m_pool = pool;

            Handle = CreateHandle();
        }

        public void Dispose()
        {
            var handles = new CBHandle[] { Handle };
            m_device.Parent.Vk.FreeCommandBuffers(m_device.LogicalDevice, m_pool.Handle, handles);
        }
        
        public CBHandle Handle { get; }

        private CBHandle CreateHandle()
        {
            var allocInfo = new CommandBufferAllocateInfo
            {
                SType = StructureType.CommandBufferAllocateInfo,
                CommandPool = m_pool.Handle,
                Level = CommandBufferLevel.Primary,
                CommandBufferCount = 1
            };

            CBHandle handle = default;

            m_device.Parent.SafeExecute(vk => vk.AllocateCommandBuffers(m_device.LogicalDevice, allocInfo, out handle));

            return handle;
        }

        #region Native Wrappers

        public void BeginRenderPass(RenderPassBeginInfo renderInfo, SubpassContents contents)
        {
            m_device.Parent.Vk.CmdBeginRenderPass(Handle, renderInfo, contents);
        }

        public void BindPipeline(PipelineBindPoint bindPoint, PLHandle pipeline)
        {
            m_device.Parent.Vk.CmdBindPipeline(Handle, bindPoint, pipeline);
        }

        public void Draw(uint vertexCount, uint instanceCount, uint firstVertex, uint firstInstance)
        {
            m_device.Parent.Vk.CmdDraw(Handle, vertexCount, instanceCount, firstVertex, firstInstance);
        }

        public void EndRenderPass()
        {
            m_device.Parent.Vk.CmdEndRenderPass(Handle);
        }

        public void End()
        {
            m_device.Parent.SafeExecute(vk => vk.EndCommandBuffer(Handle));
        }

        #endregion Native Wrappers


        public void ClearBoundTexture()
        {
        }

        public void ClearBuffers(GlobalBuffer buffers)
        {
        }

        public void DrawArrays(int vertexOffset, int vertexCount)
        {
        }

        public void DrawElements(int length)
        {
        }
    }
}
