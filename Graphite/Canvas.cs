using System.Collections.Generic;
using System.Linq;

using Tokamak;
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
    public class Canvas
    {
        private enum CallType
        {
            Debug = 0,
            DebugPoint = 1,
            Stroke = 10
        }

        private readonly List<CanvasCall> m_calls = new List<CanvasCall>(128);
        private readonly List<VectorFormatPCT> m_vectors = new List<VectorFormatPCT>(128);

        private readonly Device m_device;

        public Canvas(Device device)
        {
            m_device = device;
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
            var verts = m_device.GetVertexBuffer<VectorFormatPCT>();
            verts.Set(m_vectors);

            foreach (var call in m_calls)
                m_device.DrawArrays(PrimitiveType.TrangleStrip, call.VertexOffset, call.VertexCount);

            m_vectors.Clear();
            m_calls.Clear();
        }
    }
}
