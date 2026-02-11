using System;
using System.Collections.Generic;
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

namespace Graphite
{
    /// <summary>
    /// A 2D drawing surface.
    /// </summary>
    /// <remarks>
    /// A canvas simply builds up a list of 2D drawing commands and then sends 
    /// them along to the device in a series of buffered draw commands.
    /// 
    /// The canvas is designed to be reused between frame calls so that it does
    /// not allocate memory several times over and over again.
    /// </remarks>
    public class Canvas //: IRenderable
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

        private class CanvasCall
        {
            public PrimitiveType Type { get; set; }

            public int VertexOffset { get; set; }

            public int VertexCount { get; set; }

            public ITextureObject Texture { get; set; }
        }

        private readonly List<CanvasCall> m_calls = new List<CanvasCall>(128);
        private readonly List<VectorFormatPCT> m_vectors = new List<VectorFormatPCT>(128);

        private readonly IGraphicsLayer m_apiLayer;

        private readonly IPipeline m_pipeline;
        private readonly ICommandList m_commandList;

        private readonly IVertexBuffer<VectorFormatPCT> m_vertexBuffer;

        public Canvas(IGraphicsLayer apiLayer)
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
            m_commandList.Dispose();

            m_vertexBuffer.Dispose();

            m_pipeline.Dispose();
        }

        public Font GetFont(string filename, float size)
        {
            var dpi = m_apiLayer.GetMonitors().FirstOrDefault(m => m.IsMain)?.DPI ?? new Point(192, 192);
            //var face = m_ftLibrary.GetFace(filename, size, dpi);
            //return new Font(m_apiLayer, face);
            return null;
        }

        public void Resize(in Point size)
        {
            var mat = Matrix4x4.CreateOrthographicOffCenter(0, size.X, size.Y, 0, -1, 1);
            m_pipeline.Uniforms.projection = mat;
        }

        private void AddCall(PrimitiveType type, IEnumerable<VectorFormatPCT> vectors, ITextureObject texture = null)
        {
            var vects = vectors.ToList();

            var call = new CanvasCall
            {
                Type = type,
                VertexOffset = m_vectors.Count,
                VertexCount = vects.Count(),
                Texture = texture
            };

            m_vectors.AddRange(vects);

            m_calls.Add(call);
        }

        private VectorFormatPCT BuildVector(Pen pen, Point p, Vector2 texCoord)
        {
            return new VectorFormatPCT
            {
                Point = new Vector3(p.X, p.Y, 0),
                Color = (pen?.Color ?? Color.White).ToVector(),
                TexCoord = texCoord
            };
        }

        private VectorFormatPCT BuildVector(Pen pen, int x, int y, Vector2 texCoord)
        {
            return new VectorFormatPCT
            {
                Point = new Vector3(x, y, 0),
                Color = (pen?.Color ?? Color.White).ToVector(),
                TexCoord = texCoord
            };
        }

        public void StrokeRect(Pen pen, in Rect rect)
        {
            var stroke = new Stroke();

            stroke.Pen = pen;

            stroke.MoveTo(rect.Location);
            stroke.LineTo(rect.Right, rect.Top);
            stroke.LineTo(rect.Right, rect.Bottom);
            stroke.LineTo(rect.Left, rect.Bottom);
            stroke.Closed = true;

            var renderer = new StrokeRenderer(stroke);

            AddCall(PrimitiveType.TriangleStrip, renderer.Vectors);
        }

        // This is a simple test function for now.
        public void DrawImage(ITextureObject texture, Point p)
        {
            Vector4 color = Color.White.ToVector();

            var vects = new VectorFormatPCT[4]
            {
                new VectorFormatPCT
                {
                    Point = p,
                    Color = color,
                    TexCoord = new Vector2(0, 0),
                },
                new VectorFormatPCT
                {
                    Point = new Vector3(p.X + texture.Size.X, p.Y, 0),
                    Color = color,
                    TexCoord = new Vector2(1, 0),
                },
                new VectorFormatPCT
                {
                    Point = new Vector3(p.X, p.Y + texture.Size.Y, 0),
                    Color = color,
                    TexCoord = new Vector2(0, 1)
                },
                new VectorFormatPCT
                {
                    Point = new Vector3(p.X + texture.Size.X, p.Y + texture.Size.Y, 0),
                    Color = color,
                    TexCoord = new Vector2(1, 1)
                }
            };

            AddCall(PrimitiveType.TriangleStrip, vects, texture);
        }

        public void DrawText(Pen pen, Font font, in Point location, string text)
        {
            Point cursor = location;
            char prev = '\0';

            var glyphPolys = new List<Tuple<Glyph, VectorFormatPCT[]>>(text.Length);

            var vectors = new List<VectorFormatPCT>(text.Length * 6);

            foreach (var c in text)
            {
                Glyph g = font.GetGlyph(c);

                Vector2 tl = g.TopLeftUV;
                Vector2 br = g.BotRightUV;

                float kerning = prev != '\0' ? font.GetKerning(prev, c) : 0;

                var p = new Point(cursor.X + g.Bearing.X, cursor.Y - g.Bearing.Y);

                var detail = new Tuple<Glyph, VectorFormatPCT[]>(g, 
                    [
                        BuildVector(pen, p.X, p.Y, tl),
                        BuildVector(pen, p.X, p.Y + g.Size.Y, new Vector2(tl.X, br.Y)),
                        BuildVector(pen, p.X + g.Size.X, p.Y, new Vector2(br.X, tl.Y)),
                        BuildVector(pen, p.X, p.Y + g.Size.Y, new Vector2(tl.X, br.Y)),
                        BuildVector(pen, p.X + g.Size.X, p.Y, new Vector2(br.X, tl.Y)),
                        BuildVector(pen, p.X + g.Size.X, p.Y + g.Size.Y, br)
                    ]
                );

                glyphPolys.Add(detail);

                cursor.X += g.Size.X + g.Bearing.X;

                prev = c;
            }

            var grouped =
                from gPoly in glyphPolys
                group gPoly by gPoly.Item1.SheetNumber into grp
                select grp
            ;

            foreach (var grp in grouped)
            {
                var texture = font.GetSheet(grp.Key);
                var grpVects = grp.SelectMany(i => i.Item2).ToList();

                AddCall(PrimitiveType.TriangleList, grpVects, texture);
            }
        }

        public void Render()
        {
            ITextureObject last = null;

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
