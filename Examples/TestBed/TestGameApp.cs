using System;
using System.Collections.Generic;
using System.Numerics;
using System.IO;

using Tokamak.Hosting.Abstractions;

using Tokamak.Mathematics;

//using Tokamak.Scenes;

using Tokamak.Tritium.APIs;

using Tokamak.Graphite;
using Tokamak.Quill;

using TestBed.Scenes;

using TTF = Tokamak.Quill.Readers.TTF;

namespace TestBed
{
    public class TestGameApp : IGameApp
    {
        private readonly IAPILayer m_apiLayer;

        private const float ROT_AMOUNT = 1;//0.5f;

        private Context m_context = null;
        //private Font m_font = null;
        private Scene m_scene = null;

        //private readonly List<IRenderable> m_renderers = new List<IRenderable>();

        private TestObject m_test;

        private int m_frameCount;
        private DateTime m_lastCheck = DateTime.UtcNow;
        private float m_fps;
        private float m_rot;

        public TestGameApp(IAPILayer layer)
        {
            m_apiLayer = layer;
        }

        public void Dispose()
        {
            if (m_test != null)
            {
                m_scene.RemoveObject(m_test);
                m_test.Dispose();
            }

            m_scene?.Dispose();
            //m_font?.Dispose();
            m_context?.Dispose();
        }

        public void OnShutdown()
        {
        }

        public void OnLoad()
        {
            m_context = new Context(m_apiLayer);

            //string path = Path.Combine(Environment.SystemDirectory, "../Fonts/arial.ttf");
            //string path = Path.Combine(Environment.SystemDirectory, "../Fonts/dnk.ttf");
            string path = System.IO.Path.Combine(Environment.SystemDirectory, "../Fonts/segoeui.ttf");

            //m_font = m_context.GetFont(path, 12);

            m_scene = new Scene(m_apiLayer);
            m_test = new TestObject(m_apiLayer);

            m_scene.AddObject(m_test);

            m_scene.Camera.Location = new Vector3(0, 0, 3);
            m_scene.Camera.LookAt = Vector3.Zero;

            //m_renderers.Add(m_scene);
            //m_renderers.Add(m_canvas);
        }

        public void OnRender(double timeDelta)
        {
            RenderUI();

            m_scene.Render();
            m_context.Render();

            /*
            foreach (var r in m_renderers)
                r.Render();
            */
        }

        private void RenderUI()
        {
            PathTest();

            //DrawSingleSquare();

            //DrawFrameRate();
        }

        private void PathTest()
        {
            Pen pen = new Pen
            {
                Width = 20,
                Color = Color.Grey
                //Color = Color.White
                //Color = Color.DarkGreen
                //, LineJoin = LineJoin.Bevel
            };

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
            path.MoveTo(50, 50);
            path.BezierCubicCurveTo(
                new Vector2(200, 200),
                new Vector2(1000, 275),
                new Vector2(250, 400)
            );

            m_context.Stroke(path, pen);
        }

        //private void DrawSingleSquare()
        //{
        //    Vector2[] points = [
        //        new Vector2( 10,  10),  // Top Left
        //        new Vector2(100,  10),  // Top Right
        //        new Vector2( 10, 100),  // Bottom Left
        //        new Vector2(100, 100)   // Bottom Right
        //    ];

        //    m_context.Draw(Tokamak.Tritium.Geometry.PrimitiveType.TriangleStrip, points, Color.White);
        //}

        /*
        private void DrawFrameRate()
        {
            m_canvas.DrawText(pen, m_font, new Point(5, 30), String.Format("FPS: {0:000.0}", m_fps));
            m_canvas.DrawText(pen, m_font, new Point(5, 60), String.Format("ROT: {0:0.000}", m_rot));
        }
        */

        public void OnUpdate(double timeDelta)
        {
            ComputeFPS();

            //if (KeyboardState.IsKeyDown(Keys.Escape))
            //    Close();

            /*
            if (KeyboardState.IsKeyReleased(Keys.W))
                m_render.WireFrame = !m_render.WireFrame;

            if (KeyboardState.IsKeyReleased(Keys.D))
                m_render.Debug = !m_render.Debug;
            */

            //if (KeyboardState.IsKeyReleased(Keys.T))
            //    m_trans = !m_trans;

            m_rot += (float)(ROT_AMOUNT * timeDelta);
            //m_rot += 1;

            while (m_rot >= 360)
                m_rot -= 360;

            m_test.Rotation = new Vector3(0, m_rot, 0);
        }

        private void ComputeFPS()
        {
            ++m_frameCount;

            if (m_lastCheck.Second != DateTime.UtcNow.Second)
            {
                var diff = DateTime.UtcNow - m_lastCheck;
                m_fps = (float)(m_frameCount / diff.TotalSeconds);
                m_frameCount = 0;
                m_lastCheck = DateTime.UtcNow;
            }
        }
    }
}
