using Stashbox;

namespace Tokamak.Tritium.APIs.NullRender
{
    internal class NullAPI : IAPIDescriptor
    {
        private readonly IDependencyResolver m_resolver;

        public NullAPI(IDependencyResolver resolver)
        {
            m_resolver = resolver;
        }

        public string ID => "null";

        public string Name => "Null Renderer";

        public SupportLevel SupportLevel => SupportLevel - 100; // Should never automatically choose this.

        public IAPILayer Create() => m_resolver.Activate<NullLayer>();
    }
}
