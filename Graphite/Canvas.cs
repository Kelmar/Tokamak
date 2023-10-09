using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;

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

void main()
{
    fsout_Color = fsin_Color;
}
";

        private enum CallType
        {
            Debug = 0,
            DebugPoint = 1,
            Stroke = 10
        }

        private readonly List<CanvasCall> m_calls = new List<CanvasCall>(128);
        private readonly List<VectorFormatPCT> m_vectors = new List<VectorFormatPCT>(128);

        private readonly Device m_device;
        private readonly IShader m_shader;
        private readonly IVertexBuffer<VectorFormatPCT> m_vertexBuffer;

        public Canvas(Device device)
        {
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
        }

        public void SetSize(int width, int height)
        {
            var mat = Matrix4x4.CreateOrthographicOffCenter(0, width, height, 0, -1, 1);

            m_shader.Activate();
            m_shader.Set("projection", mat);
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

            var call = new CanvasCall
            {
                VertexOffset = m_vectors.Count,
                VertexCount = renderer.Vectors.Count(),
            };

            m_vectors.AddRange(renderer.Vectors);

            m_calls.Add(call);
        }

        public void Flush()
        {
            m_shader.Activate();

            m_vertexBuffer.Set(m_vectors);

            foreach (var call in m_calls)
                m_device.DrawArrays(PrimitiveType.TrangleStrip, call.VertexOffset, call.VertexCount);

            m_vectors.Clear();
            m_calls.Clear();
        }
    }
}
