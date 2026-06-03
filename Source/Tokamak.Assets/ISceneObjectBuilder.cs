using System;
using System.Collections.Generic;
using System.Text;

namespace Tokamak.Assets
{
    public interface ISceneObjectBuilder
    {
        public ISceneObjectBuilder WithName(string name);

        public ISceneObjectBuilder AddMeshes(params IEnumerable<string> names);
    }
}
