using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Stashbox;

using Tokamak;
using Tokamak.Buffer;
using Tokamak.Core;
using Tokamak.Formats;
using Tokamak.Mathematics;
using Tokamak.Scenes;

using Tokamak.Tritium.APIs;

using Graphite;
using System.IO;

namespace TestBed
{
    public class TestGameApp : IGameApp
    {
        private readonly IAPILayer m_apiLayer;

        //private const string FRAG_SHADER = "resources/frag.spv";
        //private const string VERT_SHADER = "resources/vert.spv";

        private const string FRAG_SHADER = "../../../shaders/test.frag";
        private const string VERT_SHADER = "../../../shaders/test.vert";

        private const float ROT_AMOUNT = 0.5f;

        private Canvas m_canvas;
        private Font m_font;
        //private Scene m_scene;

        //private readonly List<IRenderable> m_renderers = new List<IRenderable>();

        //private TestObject m_test;

        private IPipeline m_pipeline;
        private ICommandList m_commandList;

        private int m_frameCount;
        private DateTime m_lastCheck = DateTime.UtcNow;
        private float m_fps;
        private float m_rot;

        public TestGameApp()
        {
            m_apiLayer = GameHost.Instance.Services.Resolve<IAPILayer>();
        }

        public void Dispose()
        {
            //m_commandList.Dispose();
            //m_pipeline.Dispose();

            //m_scene.Dispose();
            //m_font.Dispose();
            //m_canvas.Dispose();
        }

        public void OnShutdown()
        {
        }

        public void OnLoad()
        {
#if false
            m_canvas = new Canvas(m_platform);

            using var shaderFact = m_platform.GetShaderFactory();

            //string path = Path.Combine(Environment.SystemDirectory, "../Fonts/arial.ttf");
            //string path = Path.Combine(Environment.SystemDirectory, "../Fonts/dnk.ttf");
            string path = Path.Combine(Environment.SystemDirectory, "../Fonts/segoeui.ttf");
            m_font = m_canvas.GetFont(path, 12);

            //m_scene = new Scene(m_platform);
            //m_test = new TestObject(m_device);

            //m_scene.AddObject(m_test);
            //m_scene.Camera.Location = new System.Numerics.Vector3(0, 0, 10);

            //m_renderers.Add(m_scene);
            //m_renderers.Add(m_canvas);
#endif

#if false

            m_pipeline = m_platform.GetPipeline(cfg =>
            {
                cfg.UseInputFormat<VectorFormatP>();

                cfg.UseShader(ShaderType.Fragment, FRAG_SHADER);
                cfg.UseShader(ShaderType.Vertex, VERT_SHADER);

                cfg.UseCulling(CullMode.Back);
                cfg.UsePrimitive(PrimitiveType.TriangleList);
            });

            m_commandList = m_platform.GetCommandList();
#endif
        }

        public void OnRender(double timeDelta)
        {
#if false
            m_commandList.Pipeline = m_pipeline;

            m_commandList.Begin();

            m_commandList.ClearBuffers(GlobalBuffer.ColorBuffer | GlobalBuffer.DepthBuffer);

            m_commandList.DrawArrays(0, 3);

            m_commandList.End();
#endif

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
    }
}
