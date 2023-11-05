using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tokamak.Vulkan
{
    internal class ShaderFactory : IShaderFactory
    {
        private readonly Platform m_platform;

        public ShaderFactory(VkPlatform platform)
        {
            m_platform = platform;
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
