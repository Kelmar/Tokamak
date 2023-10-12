﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;

using FreeTypeWrapper;

using Tokamak;
using Tokamak.Buffer;
using Tokamak.Formats;
using Tokamak.Mathematics;

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
    public class Canvas : IDisposable
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

        private readonly FTLibrary m_ftLibrary;

        private readonly List<CanvasCall> m_calls = new List<CanvasCall>(128);
        private readonly List<VectorFormatPCT> m_vectors = new List<VectorFormatPCT>(128);

        private readonly Device m_device;
        private readonly IShader m_shader;
        private readonly IVertexBuffer<VectorFormatPCT> m_vertexBuffer;

        public Canvas(Device device)
        {
            m_ftLibrary = new FTLibrary();

            m_device = device;
            m_vertexBuffer = m_device.GetVertexBuffer<VectorFormatPCT>(BufferType.Dyanmic);

            var factory = m_device.GetShaderFactory();

            factory.AddShaderSource(ShaderType.Vertex, VERTEX);
            factory.AddShaderSource(ShaderType.Fragment, FRAGMENT);

            m_shader = factory.Build();
        }

        public void Dispose()
        {
            m_shader?.Dispose();
            m_vertexBuffer.Dispose();

            m_ftLibrary.Dispose();
        }

        public Font GetFont(string filename, float size)
        {
            var dpi = m_device.Monitors.FirstOrDefault()?.DPI ?? new Point(192, 192);
            var face = m_ftLibrary.GetFace(filename, size, dpi);
            return new Font(m_device, face);
        }

        public void SetSize(int width, int height)
        {
            var mat = Matrix4x4.CreateOrthographicOffCenter(0, width, height, 0, -1, 1);

            m_shader.Activate();
            m_shader.Set("projection", mat);
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
                Color = (Vector4)(pen?.Color ?? Color.White),
                TexCoord = texCoord
            };
        }

        private VectorFormatPCT BuildVector(Pen pen, int x, int y, Vector2 texCoord)
        {
            return new VectorFormatPCT
            {
                Point = new Vector3(x, y, 0),
                Color = (Vector4)(pen?.Color ?? Color.White),
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

            AddCall(PrimitiveType.TrangleStrip, renderer.Vectors);
        }

        // This is a simple test function for now.
        public void DrawImage(ITextureObject texture, Point p)
        {
            Vector4 color = (Vector4)Color.White;

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

            AddCall(PrimitiveType.TrangleStrip, vects, texture);
        }

        public void DrawText(Font font, in Point location, string text)
        {
            Point cursor = location;
            char prev = '\0';

            foreach (var c in text)
            {
                Glyph g = font.GetGlyph(c);

                Vector2 tl = g.TopLeftUV;
                Vector2 br = g.BotRightUV;

                float kerning = prev != '\0' ? font.GetKerning(prev, c) : 0;

                Pen pen = null;

                var texture = font.GetSheet(g.SheetNumber);

                var p = new Point(cursor.X + g.Bearing.X, cursor.Y - g.Bearing.Y);

                var vectors = new VectorFormatPCT[4]
                {
                    BuildVector(pen, p.X, p.Y, tl),
                    BuildVector(pen, p.X, p.Y + g.Size.Y, new Vector2(tl.X, br.Y)),
                    BuildVector(pen, p.X + g.Size.X, p.Y, new Vector2(br.X, tl.Y)),
                    BuildVector(pen, p.X + g.Size.X, p.Y + g.Size.Y, br)
                };

                AddCall(PrimitiveType.TrangleStrip, vectors, texture);

                cursor.X += g.Size.X + g.Bearing.X;

                prev = c;
            }
        }

        public void Flush()
        {
            ITextureObject last = null;

            m_shader.Activate();

            m_vertexBuffer.Set(m_vectors);

            foreach (var call in m_calls)
            {
                //if (call.Texture != last)
                {
                    if (call.Texture != null)
                    {
                        if (call.Texture.Format == PixelFormat.FormatA8)
                            m_shader.Set("is8Bit", 1);
                        else
                            m_shader.Set("is8Bit", 0);

                        call.Texture.Activate();
                    }
                    else
                        m_device.ClearBoundTexture();

                    last = call.Texture;
                }

                m_device.DrawArrays(call.Type, call.VertexOffset, call.VertexCount);
            }

            //if (last != null)
            {
                m_shader.Set("is8Bit", 0);
                m_device.ClearBoundTexture();
            }

            m_vectors.Clear();
            m_calls.Clear();
        }
    }
}
