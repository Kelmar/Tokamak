using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Numerics;
using System.Text;

using Tokamak.Graphite;
using Tokamak.Mathematics;

using Tokamak.Quill;

using Tokamak.Tritium.APIs;

using Tokamak.Readers.SVG;

using TTF = Tokamak.Quill.Readers.TTF;

using Silk.NET.Vulkan;

namespace TestBed
{
    internal class UITests : IDisposable
    {
        private readonly IGraphicsLayer m_gfxLayer;
        private readonly Canvas m_canvas;

        private readonly Font m_font;

        private int m_frameCount;
        private DateTime m_lastCheck = DateTime.UtcNow;
        private float m_fps;

        public UITests(IGraphicsLayer gfxLayer)
        {
            m_gfxLayer = gfxLayer;
            m_canvas = new Canvas(m_gfxLayer);

            m_font = LoadFont();
        }

        public void Dispose()
        {
            GC.SuppressFinalize(this);

            m_canvas.Dispose();
        }

        private Font LoadFont()
        {
            //string path = Path.Combine(Environment.SystemDirectory, "../Fonts/arial.ttf");
            //string path = Path.Combine(Environment.SystemDirectory, "../Fonts/dnk.ttf");
            string path = System.IO.Path.Combine(Environment.SystemDirectory, "../Fonts/segoeui.ttf");

            using var s = File.OpenRead(path);

            var monitor = m_gfxLayer.GetMonitors().FirstOrDefault();
            var dpi = monitor?.DPI ?? new Point(72, 72);

            return TTF.Reader.LoadFrom(s, 120, dpi);
        }

        public void OnUpdate()
        {
            ComputeFPS();
        }

        private void RenderUI()
        {
            PathTest();
            PacTest();

            FontTest();

            //DrawSingleSquare();

            DrawFrameRate();
        }

        private void PathTest()
        {
            var pen = new Pen
            {
                Width = 20,
                //Color = Color.Grey
                Color = new Color(192, 192, 192, 192)
                //Color = Color.White
                //Color = Color.DarkGreen
                //, LineJoin = LineJoin.Bevel
            };

            var fill = new Pen
            {
                Width = 1,
                Color = new Color(0, 0, 192, 192)
            };

            var window = new Tokamak.Graphite.Path();
            // Rectangle Test
            //path.Rectangle(new Vector2(50, 50), new Vector2(1000, 1000));
            window.RoundRect(new Vector2(50, 50), new Vector2(1000, 1000), 50);

            var path = new Tokamak.Graphite.Path();

            // Line Test
            //path.MoveTo(10, 10);
            //path.LineTo(1000, 1000);

            // Triangle Test
            //path.MoveTo(50, 50);
            //path.LineTo(250, 50);
            //path.LineTo(450, 50);
            //path.LineTo(450, 250);
            //path.LineTo(450, 450);
            //path.LineTo(250, 250);
            //path.Close();

            // Complex Test 1
            //path.MoveTo(40, 40);   // 1
            //path.LineTo(80, 30);   // 2
            //path.LineTo(80, 60);   // 3
            //path.LineTo(125, 33);  // 4
            //path.LineTo(115, 100); // 5
            //path.LineTo(50, 120);  // 6
            //path.LineTo(70, 150);  // 7

            // Complex Test 2
            //path.MoveTo(40, 40);
            //path.LineTo(80, 30);
            //path.LineTo(80, 60);
            //path.LineTo(125, 33);
            //path.LineTo(115, 100);
            //path.LineTo(50, 120);
            //path.LineTo(30, 100);
            //path.Close();

            // Complex Test 2 (scaled)
            //float scale = 4;
            //path.MoveTo(new Vector2(40, 40) * scale);
            //path.LineTo(new Vector2(80, 30) * scale);
            //path.LineTo(new Vector2(80, 60) * scale);
            //path.LineTo(new Vector2(125, 33) * scale);
            //path.LineTo(new Vector2(115, 100) * scale);
            //path.LineTo(new Vector2(50, 120) * scale);
            //path.LineTo(new Vector2(30, 100) * scale);
            //path.Close();

            // Quadradic Bezier Curve Test
            //path.MoveTo(50, 400);
            //path.BezierQuadradicCurveTo(
            //    new Vector2(250, 50),
            //    new Vector2(500, 400)
            //);

            // Cubic Bezier Curve Test
            //path.MoveTo(50, 50);
            //path.BezierCubicCurveTo(
            //    new Vector2(200, 200),
            //    new Vector2(1000, 275),
            //    new Vector2(250, 400)
            //);

            //m_canvas.Stroke(path, pen);

            // Arc Test
            path.ArcTo(new Vector2(300, 500), new Vector2(100, 250), 0, MathF.Tau);
            path.Close();

            path.ArcTo(new Vector2(300, 500), 50, 0, MathF.Tau);
            path.Close();

            m_canvas!.Fill(window, fill);

            m_canvas.Stroke(path, pen);

            var path2 = new Tokamak.Graphite.Path();
            path2.ArcTo(new Vector2(300, 500), 50, 0, MathF.Tau);
            path2.Close();

            var pen2 = new Pen
            {
                Color = Color.DarkRed,
                Width = 10
            };

            m_canvas.Stroke(path2, pen2);
        }

        private void PacTest()
        {
            var fillPen = new Pen
            {
                Width = 3,
                //Color = Color.Yellow
                Color = new Color(192, 192, 0, 255)
                //Color = new Color(255, 255, 0, 64)
            };

            var outlinePen = new Pen
            {
                Width = 3,
                Color = Color.Yellow
            };

            var pac = new Tokamak.Graphite.Path();

            Vector2 center = new(160, 160);
            float radius = 50;

            float start = MathX.Deg2Rad(270);
            float m1 = start + MathX.Deg2Rad(45);
            float m2 = (m1 + MathX.Deg2Rad(90)) - (MathF.PI * 2);

            // Draw a PacMan like shape
            //pac.MoveTo(110, 110);
            pac.ArcTo(center, radius, start, m1);
            pac.LineTo(center);

            Vector2 m2v = new(MathF.Cos(m2) * radius, MathF.Sin(m2) * radius);

            pac.LineTo(center + m2v);
            pac.ArcTo(center, radius, m2, start);
            pac.Close();

            m_canvas.Fill(pac, fillPen);
            m_canvas.Stroke(pac, outlinePen);
        }

        private void FontTest()
        {
            var fillPen = new Pen
            {
                Width = 1,
                Color = Color.White
            };

            //m_canvas.DrawText(new Vector2(50, 50), "G", m_font, fillPen);
            //m_canvas.DrawText(new Vector2(50, 50), "A", m_font, fillPen);
            m_canvas.DrawText(new Vector2(50, 50), "C", m_font!, fillPen);
        }

        //private void DrawSingleSquare()
        //{
        //    Vector2[] points = [
        //        new Vector2( 10,  10),  // Top Left
        //        new Vector2(100,  10),  // Top Right
        //        new Vector2( 10, 100),  // Bottom Left
        //        new Vector2(100, 100)   // Bottom Right
        //    ];

        //    m_canvas.Draw(Tokamak.Tritium.Geometry.PrimitiveType.TriangleStrip, points, Color.White);
        //}

        private void ComputeFPS()
        {
            ++m_frameCount;

            if (m_lastCheck.Second != DateTime.UtcNow.Second)
            {
                var diff = DateTime.UtcNow - m_lastCheck;
                m_fps = (float)(m_frameCount / diff.TotalSeconds);
                m_frameCount = 0;
                m_lastCheck = DateTime.UtcNow;

                //int secs = (int)diff.TotalSeconds;
                //if ((secs % 10) == 0)
                //    m_log.Debug("FPS: {0}", m_fps);
            }
        }

        private void DrawFrameRate()
        {
            var pen = new Pen
            {
                Width = 1,
                Color = Color.White
            };

            string str = String.Format("FPS: {0:000.0}", m_fps);
            m_canvas.DrawText(new Vector2(5, 50), str, m_font, pen);
        }
    }
}
