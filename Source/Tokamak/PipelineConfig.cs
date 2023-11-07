using System.Collections.Generic;
using System.Linq;

using Tokamak.Formats;

namespace Tokamak
{
    public class PipelineConfig
    {
        private readonly List<ShaderInfo> m_shaders = new();

        internal PipelineConfig()
        {
        }

        public IEnumerable<ShaderInfo> Shaders => m_shaders;

        public VectorFormat.Info InputFormat { get; private set; }

        public PrimitiveType Primitive { get; private set; } = PrimitiveType.TriangleList;

        public CullMode Culling { get; private set; } = CullMode.None;

        public void UseShader(ShaderType type, string path)
        {
            if (m_shaders.Any(s => s.Path == path))
                return;

            m_shaders.Add(new ShaderInfo
            {
                Type = type,
                Path = path
            });
        }

        public void UseCulling(CullMode mode)
        {
            Culling = mode;
        }

        public void UsePrimitive(PrimitiveType primitive)
        {
            Primitive = primitive;
        }

        public void UseInputFormat<T>()
            where T : struct
        {
            InputFormat = VectorFormat.GetLayoutOf<T>();
        }
    }
}
