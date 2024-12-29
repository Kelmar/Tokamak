using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Tokamak.Tritium.APIs;
using Tokamak.Tritium.Scene;

namespace Tokamak.Tritium.Rendering
{
    public class DiffusePass : RenderPass
    {
        public DiffusePass(IAPILayer apiLayer, SceneManager scene)
            : base(apiLayer, scene)
        {
        }

        protected override bool ObjectFilter(SceneObject obj) => !obj.Flags.HasFlag(SceneObjectFlag.Environmental);

        protected override void PreparePass(ICommandList cmdList)
        {
            //cmdList.EnableLighting();

            //pipeline.Uniforms.projection = Scene.Camera.GetProjectionMatrix();
            //pipeline.Uniforms.view = Scene.Camera.GetViewMatrix();
        }
    }
}
