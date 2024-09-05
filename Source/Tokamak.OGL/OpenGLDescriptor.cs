using Stashbox;

using Tokamak.Tritium;
using Tokamak.Tritium.APIs;

namespace Tokamak.OGL
{
    internal class OpenGLDescriptor : IAPIDescriptor
    {
        private readonly IDependencyResolver m_resolver;

        public OpenGLDescriptor(IDependencyResolver resolver)
        {
            m_resolver = resolver;

            // TODO: Actually check to see how OGL is implemented on this platform.
            SupportLevel = SupportLevel.Native;
        }

        public string ID => "OpenGL";

        public string Name => "OpenGL";

        public SupportLevel SupportLevel { get; }

        public IAPILayer Create() => m_resolver.Activate<OpenGLLayer>();
    }
}
