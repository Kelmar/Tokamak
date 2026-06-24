using System.Collections.Generic;

namespace Tokamak.Import.Builders
{
    public interface ISceneObjectBuilder
    {
        public ISceneObjectBuilder WithName(string name);

        public ISceneObjectBuilder AddMeshes(params IEnumerable<string> names);

        public ISceneObjectBuilder WithSkeleton(string name);
    }
}
