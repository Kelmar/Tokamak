using System;
using System.Collections.Generic;
using System.IO;

using Silk.NET.Maths;
using Silk.NET.Windowing;

using Tokamak;
using Tokamak.Buffer;
using Tokamak.Mathematics;
using Tokamak.Scenes;

using Graphite;

namespace TestBed
{
    public class MainWindow : IDisposable
    {
        private const bool USE_VULKAN = true;

        private const float ROT_AMOUNT = 0.5f;

        private readonly IWindow m_silkWindow;

        private Device m_device;
        private Canvas m_canvas;
        private Font m_font;
        private Scene m_scene;

        private readonly List<IRenderable> m_renderers = new List<IRenderable>();

        private TestObject m_test;

        private int m_frameCount;
        private DateTime m_lastCheck = DateTime.UtcNow;
        private float m_fps;
        private float m_rot;

        public MainWindow()
        {
            var options = WindowOptions.Default;
            options.Size = new Vector2D<int>(1920, 1080);
            options.Title = "OpenGL Test SILK!";
            options.VSync = false;

            if (USE_VULKAN)
            {
                options.API = new GraphicsAPI(ContextAPI.Vulkan, new APIVersion(1, 0));
            }
            else
            {
                options.API = new GraphicsAPI(ContextAPI.OpenGL, ContextProfile.Core, ContextFlags.ForwardCompatible, new APIVersion(4, 1));
            }

            m_silkWindow = Window.Create(options);

            m_silkWindow.Load += OnLoad;
            m_silkWindow.Closing += OnClosing;
            m_silkWindow.Update += OnUpdateFrame;
            m_silkWindow.Render += OnRenderFrame;
            m_silkWindow.Resize += OnResize;
        }

        public void Dispose()
        {
            m_silkWindow.Dispose();
        }

        private void OnLoad()
        {
            if (USE_VULKAN)
            {
                var log = new ConsoleLog<Tokamak.Vulkan.VkDevice>();
                m_device = new Tokamak.Vulkan.VkDevice(log, m_silkWindow);
            }
            else
            {
                m_device = new Tokamak.OGL.GLDevice(m_silkWindow);
            }

            m_canvas = new Canvas(m_device);

            using var shaderFact = m_device.GetShaderFactory();

            //string path = Path.Combine(Environment.SystemDirectory, "../Fonts/arial.ttf");
            //string path = Path.Combine(Environment.SystemDirectory, "../Fonts/dnk.ttf");
            //string path = "C:\\Users\\kfire\\AppData\\Local\\Microsoft\\Windows\\Fonts\\dreamscar.ttf";
            string path = Path.Combine(Environment.SystemDirectory, "../Fonts/segoeui.ttf");
            m_font = m_canvas.GetFont(path, 12);

            m_scene = new Scene(m_device);
            //m_test = new TestObject(m_device);

            //m_scene.AddObject(m_test);
            m_scene.Camera.Location = new System.Numerics.Vector3(0, 0, 10);

            m_renderers.Add(m_scene);
            m_renderers.Add(m_canvas);

            OnResize(m_silkWindow.Size);
        }

        private void OnClosing()
        {
            m_scene.Dispose();
            m_font.Dispose();
            m_canvas.Dispose();
            m_device.Dispose();
        }

        public void Run() => m_silkWindow.Run();

        protected void OnUpdateFrame(double delta)
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

            m_rot += (float)(ROT_AMOUNT * delta);

            m_test.Rotation = new System.Numerics.Vector3(0, (float)m_rot, 0);
        }

        private void ComputeFPS()
        {
            ++m_frameCount;

            if (m_lastCheck != DateTime.UtcNow)
            {
                var diff = DateTime.UtcNow - m_lastCheck;
                m_fps = (float)(m_frameCount / diff.TotalSeconds);
                m_frameCount = 0;
                m_lastCheck = DateTime.UtcNow;
            }
        }

        protected void OnRenderFrame(double delta)
        {
            m_device.ClearBuffers(GlobalBuffer.ColorBuffer | GlobalBuffer.DepthBuffer);

            Pen pen = new Pen
            {
                Width = 40,
                Color = Color.White
                //LineJoin = LineJoin.Bevel
            };

            m_canvas.DrawText(pen, m_font, new Point(5, 30), String.Format("FPS: {0:000.0}", m_fps));

            foreach (var r in m_renderers)
                r.Render();
        }

        protected void OnResize(Vector2D<int> size)
        {
            if (size.X > 0 && size.Y > 0)
            {
                m_device.Viewport = new Rect(0, 0, size.X, size.Y);

                Point s = new Point(size.X, size.Y);

                foreach (var r in m_renderers)
                    r.Resize(s);
            }
        }
    }
}
