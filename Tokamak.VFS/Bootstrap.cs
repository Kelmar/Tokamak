using Tokamak.Abstractions.Services;
using Tokamak.Abstractions.VFS;

namespace Tokamak.VFS
{
    public static class Bootstrap
    {
        public static IServiceLocator InitVFS(this IServiceLocator locator)
        {
            locator.Register<IMountSystem>(new MountSystem());
            return locator;
        }
    }
}
