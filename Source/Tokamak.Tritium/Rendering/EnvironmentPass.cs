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
    public class EnvironmentPass : RenderPass
    {
        public EnvironmentPass(IGraphicsLayer apiLayer, SceneManager scene)
            : base(apiLayer, scene)
        {
        }

        protected override void PreparePass(ICommandList cmdList)
        {
            //cmdList.DisableLighting();

            //pipeline.Uniforms.projection = Scene.Camera.GetFixedProjectionMatrix();
            //pipeline.Uniforms.view = Scene.Camera.GetViewMatrix();
        }

        protected override bool ObjectFilter(SceneObject obj) => obj.Flags.HasFlag(SceneObjectFlag.Environmental);
    }
}
