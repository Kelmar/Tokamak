using System.Collections.Generic;
using System.Linq;

using Tokamak.Mathematics;

using Tokamak.Tritium.APIs;
using Tokamak.Tritium.Buffers.Formats;

namespace Tokamak.Tritium.Pipelines
{
    public class PipelineConfig
    {
        private readonly List<ShaderInfo> m_shaders = new();

        internal PipelineConfig()
        {
        }

        public Color ClearColor { get; private set; } = Color.Black;

        public IEnumerable<ShaderInfo> Shaders => m_shaders;

        public VectorFormat.Info InputFormat { get; private set; }

        public PrimitiveType Primitive { get; private set; } = PrimitiveType.TriangleList;

        public bool DepthTest { get; private set; } = false;

        public CullMode Culling { get; private set; } = CullMode.None;

        public PipelineConfig UseShader(ShaderType type, string path)
        {
            if (m_shaders.Any(s => s.Path == path))
                return this;

            m_shaders.Add(new ShaderInfo
            {
                Type = type,
                Path = path
            });

            return this;
        }

        public PipelineConfig UseClearColor(Color color)
        {
            ClearColor = color;
            return this;
        }

        public PipelineConfig UseCulling(CullMode mode)
        {
            Culling = mode;
            return this;
        }

        public PipelineConfig UsePrimitive(PrimitiveType primitive)
        {
            Primitive = primitive;
            return this;
        }

        public PipelineConfig UseInputFormat<T>()
            where T : struct
        {
            InputFormat = VectorFormat.GetLayoutOf<T>();
            return this;
        }

        public PipelineConfig EnableDepthTest(bool value = true)
        {
            DepthTest = value;
            return this;
        }
    }
}
