using Stashbox;

using Tokamak.VFS.Abstractions;

namespace Tokamak.VFS
{
    public static class Bootstrap
    {
        public static IStashboxContainer InitVFS(this IStashboxContainer locator)
        {
            locator.RegisterInstance<IMountSystem>(new MountSystem());
            return locator;
        }
    }
}
