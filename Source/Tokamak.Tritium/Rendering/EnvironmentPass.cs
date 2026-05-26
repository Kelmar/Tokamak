using Tokamak.Tritium.APIs;
using Tokamak.Tritium.Pipelines;
using Tokamak.Tritium.Scene;

namespace Tokamak.Tritium.Rendering
{
    public class EnvironmentPass : RenderPass
    {
        public EnvironmentPass(IGraphicsLayer gfxLayer, SceneManager scene)
            : base(gfxLayer, scene)
        {
        }

        protected override void PreparePass(IPipeline pipeline, ICommandList commandList)
        {
            //commandList.DisableLighting();

            pipeline.Uniforms.projection = Scene.Camera.GetFixedProjectionMatrix();
            pipeline.Uniforms.view = Scene.Camera.GetViewMatrix();
        }

        protected override bool ObjectFilter(SceneObject obj) => obj.Flags.HasFlag(SceneObjectFlag.Environmental);
    }
}
