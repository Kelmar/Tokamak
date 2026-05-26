using System.IO;

using Tokamak.Tritium.APIs;
using Tokamak.Tritium.Buffers.Formats;
using Tokamak.Tritium.Geometry;
using Tokamak.Tritium.Pipelines;
using Tokamak.Tritium.Pipelines.Shaders;
using Tokamak.Tritium.Scene;

namespace TestBed.Scenes
{
    public class SceneInitializer : ISceneInitializer
    {
        private readonly IGraphicsLayer m_gfxLayer;

        public SceneInitializer(IGraphicsLayer gfxLayer)
        {
            m_gfxLayer = gfxLayer;
        }

        public IPipeline GetPipeline()
        {
            string vertexShader = File.ReadAllText("shaders/basic.vert");
            string fragmentShader = File.ReadAllText("shaders/basic.frag");

            return m_gfxLayer.CreatePipeline(cfg =>
            {
                cfg.UseInputFormat<VectorFormatPNCT>();

                cfg.UseCulling(CullMode.None);
                cfg.EnableDepthTest(true);
                cfg.UsePrimitive(PrimitiveType.TriangleList);

                cfg.AddShaderCode(ShaderType.Vertex, vertexShader);
                cfg.AddShaderCode(ShaderType.Fragment, fragmentShader);
            });
        }

        public ICommandList GetCommandList()
        {
            return m_gfxLayer.CreateCommandList();
        }
    }
}
