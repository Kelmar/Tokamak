using Stashbox;

using Tokamak.Tritium.APIs;

namespace Tokamak.OGL
{
    public static class Bootstrap
    {
        public static IStashboxContainer AllowOpenGL(this IStashboxContainer container)
        {
            container.RegisterSingleton<IGraphicsDescriptor, OpenGLDescriptor>();

            return container;
        }
    }
}
