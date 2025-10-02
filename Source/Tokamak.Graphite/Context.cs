using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Numerics;
using System.Runtime.InteropServices;

using Tokamak.Mathematics;

using Tokamak.Tritium.APIs;

using Tokamak.Tritium.Buffers;
using Tokamak.Tritium.Buffers.Formats;

using Tokamak.Tritium.Geometry;

using Tokamak.Tritium.Pipelines;
using Tokamak.Tritium.Pipelines.Shaders;

namespace Tokamak.Graphite
{
    public class Context : IDisposable//, IRenderable
    {
        // For now we have some fairly basic shaders for testing the canvas out.

        public const string VERTEX = @"#version 450

uniform mat4 projection;

layout(location = 0) in vec3 Point;
layout(location = 1) in vec4 Color;
layout(location = 2) in vec2 TexCoord;

layout(location = 0) out vec4 fsin_Color;
layout(location = 1) out vec2 fsin_TexCoord;

void main()
{
    gl_Position = vec4(Point, 1.0) * projection;
    fsin_Color = Color;
    fsin_TexCoord = TexCoord;
}
";

        public const string FRAGMENT = @"#version 450

layout(location = 0) in vec4 fsin_Color;
layout(location = 1) in vec2 fsin_TexCoord;

layout(location = 0) out vec4 fsout_Color;

uniform int is8Bit;
uniform sampler2D texture0;

void main()
{
    vec4 tx = texture(texture0, fsin_TexCoord);

    fsout_Color = is8Bit != 0 ? vec4(fsin_Color.rgb, fsin_Color.a * tx.r) : tx * fsin_Color;
}
";

        private class RenderCall
        {
            public PrimitiveType Type { get; set; }

            public int VertexOffset { get; set; }

            public int VertexCount { get; set; }

            public ITextureObject? Texture { get; set; }
        }

        private readonly List<RenderCall> m_calls = new (128);
        private readonly List<VectorFormatPCT> m_vectors = new (128);

        private readonly IAPILayer m_apiLayer;

        private readonly IPipeline m_pipeline;
        private readonly ICommandList m_commandList;
        private readonly IVertexBuffer<VectorFormatPCT> m_vertexBuffer;

        public Context(IAPILayer apiLayer)
        {
            m_apiLayer = apiLayer;

            m_pipeline = m_apiLayer.CreatePipeline(cfg => cfg
                .UseInputFormat<VectorFormatPCT>()
                .EnableDepthTest(false)
                .EnableBlending(
                    // A good blending function for 2D font antialiasing.
                    sourceFactor: BlendFactor.SourceAlpha,
                    destinationFactor: BlendFactor.One
                )
                .UseCulling(CullMode.None)
                .AddShaderCode(ShaderType.Vertex, VERTEX)
                .AddShaderCode(ShaderType.Fragment, FRAGMENT)
            );

            m_commandList = m_apiLayer.CreateCommandList();

            m_vertexBuffer = m_apiLayer.GetVertexBuffer<VectorFormatPCT>(BufferUsage.Dynamic);
        }

        public void Dispose()
        {
            m_vertexBuffer.Dispose();
            m_commandList.Dispose();
            m_pipeline.Dispose();
        }

        public void Resize(in Point size)
        {
            var mat = Matrix4x4.CreateOrthographicOffCenter(0, size.X, size.Y, 0, -1, 1);
            m_pipeline.Uniforms.projection = mat;
        }

        public void Draw(PrimitiveType primitiveType, IEnumerable<Vector2> vectors, Color color, ITextureObject? texture = null)
        {
            var vectorList = vectors.Select(v => BuildVectorPCT(v, color, Vector2.Zero));

            AddCall(primitiveType, vectorList, texture);
        }

        private VectorFormatPCT BuildVectorPCT(in Vector2 v, Color color, Vector2 texCoord)
        {
            return new VectorFormatPCT
            {
                Point = new Vector3(v.X, v.Y, 0),
                Color = color.ToVector(),
                TexCoord = texCoord
            };
        }

        internal void AddCall(PrimitiveType type, IEnumerable<VectorFormatPCT> vectors, ITextureObject? texture = null)
        {
            var vectorList = vectors.ToList();

            var call = new RenderCall
            {
                Type = type,
                VertexOffset = m_vectors.Count,
                VertexCount = vectorList.Count(),
                Texture = texture
            };

            m_vectors.AddRange(vectorList);

            m_calls.Add(call);
        }

        public ICanvas GetCanvas()
        {
            return new Canvas(this);
        }

        public void Render()
        {
            if (m_vectors.Count == 0 || m_calls.Count == 0)
            {
                // Nothing to do
#if DEBUG
                // Debugging sanity checks.

                if (m_vectors.Count != 0 && m_calls.Count == 0)
                    Debug.Fail("Got vectors without calls?");

                if (m_calls.Count != 0 && m_vectors.Count == 0)
                    Debug.Fail("Got calls without vectors?");
#endif

                m_vectors.Clear();
                m_calls.Clear();

                return; 
            }


            ITextureObject? last = null;

            m_vertexBuffer.Set(CollectionsMarshal.AsSpan(m_vectors));

            m_pipeline.Activate(m_commandList);

            using var cmdScope = m_commandList.BeginScope();

            Resize(m_apiLayer.ViewBounds);

            foreach (var call in m_calls)
            {
                if (call.Texture != last)
                {
                    if (call.Texture != null)
                    {
                        m_pipeline.Uniforms.is8Bit = call.Texture.Format == PixelFormat.FormatA8;
                        //call.Texture.Activate();
                    }
                    else
                    {
                        m_commandList.ClearBoundTexture();
                    }

                    last = call.Texture;
                }

                m_commandList.DrawArrays(call.VertexOffset, call.VertexCount);
            }

            if (last != null)
            {
                m_pipeline.Uniforms.is8Bit = false;
                m_commandList.ClearBoundTexture();
            }

            m_vectors.Clear();
            m_calls.Clear();
        }
    }
}
