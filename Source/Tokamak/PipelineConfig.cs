using System.Collections.Generic;
using System.Linq;

using Tokamak.Formats;

namespace Tokamak
{
    public abstract class PipelineConfig
    {
        private readonly List<ShaderInfo> m_shaders = new();

        protected PipelineConfig(VectorFormat.Info inputFormat)
        {
            InputFormat = inputFormat;
        }

        public IEnumerable<ShaderInfo> Shaders => m_shaders;

        public VectorFormat.Info InputFormat { get; }

        public PrimitiveType Primitive { get; private set; }

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
    }

    public class PipelineConfig<TInputFormat> : PipelineConfig
        where TInputFormat : struct
    {
        public PipelineConfig()
            : base(VectorFormat.GetLayoutOf<TInputFormat>())
        {
        }
    }
}
