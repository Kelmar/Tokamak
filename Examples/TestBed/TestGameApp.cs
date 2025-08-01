using System;
using System.Collections.Generic;
using System.Numerics;
using System.IO;

using Tokamak.Core;

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
        private Font m_font = null;
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
            string path = Path.Combine(Environment.SystemDirectory, "../Fonts/segoeui.ttf");

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
            Pen pen = new Pen
            {
                Width = 40,
                Color = Color.White
                //LineJoin = LineJoin.Bevel
            };

            var canvas = m_context.GetCanvas();

            canvas.MoveTo(10, 10);
            canvas.LineTo(100, 100);

            canvas.StrokeColor = Color.LiteRed;
            canvas.Stroke();

            //m_canvas.DrawText(pen, m_font, new Point(5, 30), String.Format("FPS: {0:000.0}", m_fps));
            //m_canvas.DrawText(pen, m_font, new Point(5, 60), String.Format("ROT: {0:0.000}", m_rot));
        }

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
