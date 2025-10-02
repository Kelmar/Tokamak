using Tokamak.Hosting.Abstractions;

using Tokamak.Tritium.APIs;

namespace Tokamak.OGL
{
    internal class OpenGLDescriptor : IAPIDescriptor
    {
        private readonly IHostEnvironment m_hostEnvironment;
        private readonly IGameLifetime m_gameLifetime;

        public OpenGLDescriptor(IHostEnvironment hostEnvironment, IGameLifetime gameLifetime)
        {
            m_hostEnvironment = hostEnvironment;
            m_gameLifetime = gameLifetime;

            // TODO: Actually check to see how OGL is implemented on this platform.
            SupportLevel = SupportLevel.Native;
        }

        public string ID => "OpenGL";

        public string Name => "OpenGL";

        public SupportLevel SupportLevel { get; }

        public IAPILayer Build()
        {
            return new OpenGLLayer(m_hostEnvironment, m_gameLifetime);
        }
    }
}
