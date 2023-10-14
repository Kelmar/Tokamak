using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tokamak.Vulkan
{
    internal class ShaderFactory : IShaderFactory
    {
        private readonly Device m_device;

        public ShaderFactory(VkDevice device)
        {
            m_device = device;
        }

        public void Dispose()
        {
        }

        public void AddShaderSource(ShaderType type, string source)
        {
        }

        public IShader Build()
        {
            return null;
        }
    }
}
