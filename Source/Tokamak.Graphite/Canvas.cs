using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Numerics;
using System.Runtime.InteropServices;

using Tokamak.Graphite.PathRendering;
using Tokamak.Mathematics;

using Tokamak.Tritium.APIs;

using Tokamak.Tritium.Buffers;
using Tokamak.Tritium.Buffers.Formats;

using Tokamak.Tritium.Geometry;

using Tokamak.Tritium.Pipelines;
using Tokamak.Tritium.Pipelines.Shaders;

namespace Tokamak.Graphite
{
    public class Canvas : IDisposable//, IRenderable
    {
        /*
         * For now we hard code for 100 segments, probably should
         * dynamically calculate this based on scaling in the future.
         */
        private const int PATH_RESOLUTION = 100;
        //private const int PATH_RESOLUTION = 2;


        // For now we have some fairly basic shaders for testing the canvas out.

        // TODO: Look into SPIR-V shaders, should be supported by OGL, Vulkan, and DirectX 12

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

        // TODO: Does it make sense to have a generic call list processor in Tritium?

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

        public Canvas(IAPILayer apiLayer)
        {
            m_apiLayer = apiLayer;

            m_pipeline = m_apiLayer.CreatePipeline(cfg => cfg
                .UseInputFormat<VectorFormatPCT>()
                //.UsePrimitive(PrimitiveType.TriangleStrip)
                .UsePrimitive(PrimitiveType.TriangleList)
                .EnableDepthTest(false)
                .EnableBlending(
                    sourceFactor: BlendFactor.SourceAlpha,
                    destinationFactor: BlendFactor.OneMinusSourceAlpha
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

            GC.SuppressFinalize(this);
        }

        public void Resize(in Point size)
        {
            var mat = Matrix4x4.CreateOrthographicOffCenter(0, size.X, size.Y, 0, -1, 1);
            m_pipeline.Uniforms.projection = mat;
        }

        public void DrawImage(ITextureObject texture, in Point p) =>
            DrawImage(texture, p, Color.White);

        public void DrawImage(ITextureObject texture, in Point p, in Color color)
        {
            AddCall(
                PrimitiveType.TriangleStrip,
                [
                    BuildPointPCT(p, color, Vector2.Zero),
                    BuildPointPCT(new Point(p.X + texture.Size.X, p.Y), color, Vector2.UnitX),
                    BuildPointPCT(new Point(p.X, p.Y + texture.Size.Y), color, Vector2.UnitY),
                    BuildPointPCT(p + texture.Size, color, Vector2.One)
                ],
                texture
            );
        }

        public void DrawImage(ITextureObject texture, in Point p, in Vector2 topLeftUV, in Vector2 bottomRightUV) =>
            DrawImage(texture, p, topLeftUV, bottomRightUV, Color.White);

        public void DrawImage(ITextureObject texture, in Point p, in Vector2 topLeftUV, in Vector2 bottomRightUV, in Color color)
        {
            Vector2 topRightUV = new(bottomRightUV.X, topLeftUV.Y);
            Vector2 bottomLeftUV = new(topLeftUV.X, bottomRightUV.Y);

            AddCall(
                PrimitiveType.TriangleStrip,
                [
                    BuildPointPCT(p, color, topLeftUV),
                    BuildPointPCT(new Point(p.X + texture.Size.X, p.Y), color, topRightUV),
                    BuildPointPCT(new Point(p.X, p.Y + texture.Size.Y), color, bottomLeftUV),
                    BuildPointPCT(p + texture.Size, color, bottomRightUV)
                ],
                texture
            );
        }

        public void Stroke(Path path, Pen pen)
        {
            foreach (var stroke in path.m_strokes)
            {
                var renderer = new StrokeRenderer(stroke, PATH_RESOLUTION, pen.Width);

                var points = renderer.Render().ToList();

                Draw(PrimitiveType.TriangleList, points, pen.Color);
            }
        }

        public void Fill(Path path, Pen pen)
        {
            foreach (var stroke in path.m_strokes)
            {
                var renderer = new FillRenderer(stroke, PATH_RESOLUTION);

                renderer.Render(this, pen);
            }
        }

        public void Draw(PrimitiveType primitiveType, IEnumerable<Vector2> vectors, Color color, ITextureObject? texture = null)
        {
            Debug.Assert(vectors.Any(), "No vectors in Draw() call.");

            var vectorList = vectors.Select(v => BuildVectorPCT(v, color, Vector2.Zero));

            AddCall(primitiveType, vectorList, texture);
        }

        private static VectorFormatPCT BuildPointPCT(in Point p, in Color color, in Vector2 uv)
        {
            return new VectorFormatPCT
            {
                Point = new Vector3(p.X, p.Y, 0),
                Color = color.ToVector(),
                TexCoord = uv
            };
        }

        private static VectorFormatPCT BuildVectorPCT(in Vector2 v, Color color, Vector2 uv)
        {
            return new VectorFormatPCT
            {
                Point = new Vector3(v.X, v.Y, 0),
                Color = color.ToVector(),
                TexCoord = uv
            };
        }

        internal void AddCall(PrimitiveType type, IEnumerable<VectorFormatPCT> vectors, ITextureObject? texture = null)
        {
            var vectorList = vectors.ToList();

            var call = new RenderCall
            {
                Type = type,
                VertexOffset = m_vectors.Count,
                VertexCount = vectorList.Count,
                Texture = texture
            };

            m_vectors.AddRange(vectorList);

            m_calls.Add(call);
        }

        [Conditional("DEBUG")]
        private void Render_SanityCheck()
        {
            // Debugging sanity checks.

            if (m_vectors.Count != 0 && m_calls.Count == 0)
                Debug.Fail("Got vectors without calls?");

            if (m_calls.Count != 0 && m_vectors.Count == 0)
                Debug.Fail("Got calls without vectors?");

            m_vectors.Clear();
            m_calls.Clear();
        }

        public void Render()
        {
            if (m_vectors.Count == 0 || m_calls.Count == 0)
            {
                // Nothing to do
                Render_SanityCheck();
                return; 
            }

            ITextureObject? last = null;

            m_vertexBuffer.Set(CollectionsMarshal.AsSpan(m_vectors));

            m_pipeline.Activate(m_commandList);

            using var cmdScope = m_commandList.BeginScope();

            Resize(m_apiLayer.ViewBounds); // TODO: Move this, better to be elsewhere.

            try
            {
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
            }
            finally
            {
                m_vectors.Clear();
                m_calls.Clear();
            }
        }
    }
}
