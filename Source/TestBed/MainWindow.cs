using System;
using System.Collections.Generic;
using System.IO;

using Silk.NET.Maths;
using Silk.NET.Windowing;

using Tokamak;
using Tokamak.Buffer;
using Tokamak.Config;
using Tokamak.Formats;
using Tokamak.Logging;
using Tokamak.Mathematics;
using Tokamak.Scenes;

using Graphite;

namespace TestBed
{
    public class MainWindow : IDisposable
    {
        private const string FRAG_SHADER = "resources/frag.spv";
        private const string VERT_SHADER = "resources/vert.spv";

        private const float ROT_AMOUNT = 0.5f;

        private readonly IConfigReader m_config;
        private readonly string m_driver;

        private readonly IWindow m_silkWindow;

        private Platform m_platform;

        //private Canvas m_canvas;
        //private Font m_font;
        //private Scene m_scene;

        //private readonly List<IRenderable> m_renderers = new List<IRenderable>();

        //private TestObject m_test;

        private IPipeline m_pipeline;
        private ICommandBuffer m_commandBuffer;

        private int m_frameCount;
        private DateTime m_lastCheck = DateTime.UtcNow;
        private float m_fps;
        private float m_rot;

        public MainWindow()
        {
            Platform.Services.Register<ILogFactory>(new LogFactory());
            Platform.Services.Register<IConfigReader>(new BasicConfigReader());

            m_config = Platform.Services.Find<IConfigReader>();
            m_driver = m_config.Get("Tok.Driver", "Vulkan");

            var options = WindowOptions.Default;
            options.Size = new Vector2D<int>(1920, 1080);
            options.Title = "Tokamak Test!";
            options.VSync = false;

            switch (m_driver.ToUpper())
            {
            case "VULKAN":
                options.API = new GraphicsAPI(ContextAPI.Vulkan, new APIVersion(1, 0));
                break;

            case "OPENGL":
                options.API = new GraphicsAPI(ContextAPI.OpenGL, ContextProfile.Core, ContextFlags.ForwardCompatible, new APIVersion(4, 1));
                break;

            default:
                throw new Exception($"Unknown driver: {m_driver}");
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
            switch (m_driver.ToUpper())
            {
            case "VULKAN":
                m_platform = new Tokamak.Vulkan.VkPlatform(m_silkWindow);
                break;

            case "OPENGL":
                m_platform = new Tokamak.OGL.GLPlatform(m_silkWindow);
                break;

            default:
                throw new Exception("Unknown driver");
            }

#if false
            m_canvas = new Canvas(m_platform);

            using var shaderFact = m_platform.GetShaderFactory();

            //string path = Path.Combine(Environment.SystemDirectory, "../Fonts/arial.ttf");
            //string path = Path.Combine(Environment.SystemDirectory, "../Fonts/dnk.ttf");
            string path = Path.Combine(Environment.SystemDirectory, "../Fonts/segoeui.ttf");
            m_font = m_canvas.GetFont(path, 12);

            m_scene = new Scene(m_platform);
            //m_test = new TestObject(m_device);

            //m_scene.AddObject(m_test);
            m_scene.Camera.Location = new System.Numerics.Vector3(0, 0, 10);

            m_renderers.Add(m_scene);
            m_renderers.Add(m_canvas);
#endif

            OnResize(m_silkWindow.Size);

            m_pipeline = m_platform.GetPipeline(cfg =>
            {
                cfg.UseInputFormat<VectorFormatP>();

                cfg.UseShader(ShaderType.Fragment, FRAG_SHADER);
                cfg.UseShader(ShaderType.Vertex, VERT_SHADER);

                cfg.UseCulling(CullMode.Back);
                cfg.UsePrimitive(PrimitiveType.TriangleList);
            });

            m_commandBuffer = m_platform.GetCommandBuffer();
        }

        private void OnClosing()
        {
            m_commandBuffer.Dispose();
            m_pipeline.Dispose();

            //m_scene.Dispose();
            //m_font.Dispose();
            //m_canvas.Dispose();

            m_platform.Dispose();
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

            //m_test.Rotation = new System.Numerics.Vector3(0, (float)m_rot, 0);
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
            m_pipeline.Activate(m_commandBuffer);

            m_commandBuffer.Reset();

            m_commandBuffer.Begin();

            m_commandBuffer.BeginPass();

            m_commandBuffer.ClearBuffers(GlobalBuffer.ColorBuffer | GlobalBuffer.DepthBuffer);

            m_commandBuffer.DrawArrays(0, 3);

            m_commandBuffer.EndPass();

            m_commandBuffer.End();

            m_commandBuffer.Flush();

            /*

            Pen pen = new Pen
            {
                Width = 40,
                Color = Color.White
                //LineJoin = LineJoin.Bevel
            };

            m_canvas.DrawText(pen, m_font, new Point(5, 30), String.Format("FPS: {0:000.0}", m_fps));

            foreach (var r in m_renderers)
                r.Render();
            */
        }

        protected void OnResize(Vector2D<int> size)
        {
            if (size.X > 0 && size.Y > 0)
            {
                m_platform.Viewport = new Rect(0, 0, size.X, size.Y);

                /*
                Point s = new Point(size.X, size.Y);

                foreach (var r in m_renderers)
                    r.Resize(s);
                */
            }
        }
    }
}
