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
        private const string VERTEX_SHADER = "shaders/basic.vert";
        //private const string VERTEX_SHADER = "shaders/bone.vert";

        private const string FRAGMENT_SHADER = "shaders/basic.frag";

        private readonly IGraphicsLayer m_gfxLayer;

        public SceneInitializer(IGraphicsLayer gfxLayer)
        {
            m_gfxLayer = gfxLayer;
        }

        public IPipeline GetPipeline()
        {
            string vertexShader = File.ReadAllText(VERTEX_SHADER);
            string fragmentShader = File.ReadAllText(FRAGMENT_SHADER);

            return m_gfxLayer.CreatePipeline(cfg =>
            {
                cfg.UseInputFormat<VertexFormatPNCT>();

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
