using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Tokamak.Buffer;
using Tokamak.Vulkan.NativeWrapper;

namespace Tokamak.Vulkan
{
    internal class CommandBuffer : ICommandBuffer
    {
        private readonly VkCommandBuffer m_buffer;

        public CommandBuffer(VkCommandBuffer buffer)
        {
            m_buffer = buffer;
        }

        public void Dispose()
        {
            m_buffer.Dispose();
        }

        public void ClearBoundTexture()
        {
        }

        public void ClearBuffers(GlobalBuffer buffers)
        {
        }

        public void DrawArrays(PrimitiveType primitive, int vertexOffset, int vertexCount)
        {
        }

        public void DrawElements(PrimitiveType primitive, int length)
        {
        }
    }
}
