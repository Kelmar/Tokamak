using System.Collections.Generic;

using Tokamak.Tritium.Scene;

namespace Tokamak.Tritium.Rendering
{
    public interface IRenderPass
    {
        void Render(IEnumerable<SceneObject> objects);
    }
}
