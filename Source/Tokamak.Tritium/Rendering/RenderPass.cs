using System.Collections.Generic;
using System.Linq;

using Tokamak.Tritium.APIs;
using Tokamak.Tritium.Pipelines;
using Tokamak.Tritium.Scene;

namespace Tokamak.Tritium.Rendering
{
    public abstract class RenderPass : IRenderPass
    {
        private readonly IGraphicsLayer m_gfxLayer;

        protected RenderPass(IGraphicsLayer gfxLayer, SceneManager scene)
        {
            m_gfxLayer = gfxLayer;
            Scene = scene;
        }

        public SceneManager Scene { get; }

        abstract protected void PreparePass(IPipeline pipeline, ICommandList cmdList);

        virtual protected bool ObjectFilter(SceneObject obj) => true;

        virtual public void Render(IPipeline pipeline, ICommandList commandList, IEnumerable<SceneObject> objects)
        {
            PreparePass(pipeline, commandList);

            foreach (var obj in objects.Where(ObjectFilter))
            {
                pipeline.Uniforms.model = obj.Model;
                obj.Render(commandList);
            }
        }
    }
}
