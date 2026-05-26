using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Tokamak.Tritium.APIs;
using Tokamak.Tritium.Pipelines;
using Tokamak.Tritium.Scene;

namespace Tokamak.Tritium.Rendering
{
    public class DiffusePass : RenderPass
    {
        public DiffusePass(IGraphicsLayer gfxLayer, SceneManager scene)
            : base(gfxLayer, scene)
        {
        }

        protected override bool ObjectFilter(SceneObject obj) => !obj.Flags.HasFlag(SceneObjectFlag.Environmental);

        protected override void PreparePass(IPipeline pipeline, ICommandList commandList)
        {
            //commandList.EnableLighting();

            pipeline.Uniforms.projection = Scene.Camera.GetProjectionMatrix();
            pipeline.Uniforms.view = Scene.Camera.GetViewMatrix();
        }
    }
}
