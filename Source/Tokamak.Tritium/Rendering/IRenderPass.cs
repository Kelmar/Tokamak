using System.Collections.Generic;

using Tokamak.Tritium.APIs;
using Tokamak.Tritium.Pipelines;
using Tokamak.Tritium.Scene;

namespace Tokamak.Tritium.Rendering
{
    public interface IRenderPass
    {
        void Render(IPipeline pipeline, ICommandList commandList, IEnumerable<SceneObject> objects);
    }
}
