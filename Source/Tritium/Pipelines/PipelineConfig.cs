using System.Collections.Generic;
using System.Linq;

using Tokamak.Mathematics;
using Tokamak.Tritium.Buffers.Formats;
using Tokamak.Tritium.Pipelines.Shaders;

namespace Tokamak.Tritium.Pipelines
{
    public class PipelineConfig
    {
        private readonly List<IShaderSource> m_shaders = new();

        internal PipelineConfig()
        {
        }

        public Color ClearColor { get; private set; } = Color.Black;

        public IEnumerable<IShaderSource> ShaderSources => m_shaders;

        public VectorFormat.Info InputFormat { get; private set; }

        public PrimitiveType Primitive { get; private set; } = PrimitiveType.TriangleList;

        public bool Blending { get; private set; } = false;

        public BlendFactor SourceBlendFactor { get; private set; }

        public BlendFactor DestinationBlendFactor { get; private set; }

        public bool DepthTest { get; private set; } = false;

        public CullMode Culling { get; private set; } = CullMode.None;

        public PipelineConfig AddShaderSource(IShaderSource source)
        {
            m_shaders.Add(source);

            return this;
        }

        public PipelineConfig AddShaderFile(ShaderType type, string path)
        {
            AddShaderSource(new ShaderSourceFile(type, path));
            return this;
        }

        public PipelineConfig AddShaderCode(ShaderType type, string code)
        {
            AddShaderSource(new ShaderSourceCode(type, code));
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

        public PipelineConfig DisableBlending()
        {
            Blending = false;
            return this;
        }

        public PipelineConfig EnableBlending(
            BlendFactor sourceFactor,
            BlendFactor destinationFactor)
        {
            Blending = true;
            SourceBlendFactor = sourceFactor;
            DestinationBlendFactor = destinationFactor;

            return this;
        }
    }
}
