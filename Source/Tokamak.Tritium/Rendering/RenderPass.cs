using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Tokamak.Tritium.APIs;
using Tokamak.Tritium.Scene;

namespace Tokamak.Tritium.Rendering
{
    public abstract class RenderPass : IRenderPass
    {
        private readonly IAPILayer m_apiLayer;

        protected RenderPass(IAPILayer apiLayer, SceneManager scene)
        {
            m_apiLayer = apiLayer;
            Scene = scene;
        }

        public SceneManager Scene { get; }

        abstract protected void PreparePass(ICommandList cmdList);

        virtual protected bool ObjectFilter(SceneObject obj) => true;

        virtual public void Render(IEnumerable<SceneObject> objects)
        {
            ICommandList cmdList = m_apiLayer.CreateCommandList();

            PreparePass(cmdList);

            foreach (var obj in objects.Where(ObjectFilter))
            {
                //pipeline.Uniforms.model = obj.Model;
                obj.Render(cmdList);
            }
        }
    }
}
