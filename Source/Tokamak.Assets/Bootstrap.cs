using Stashbox;

namespace Tokamak.Assets
{
    public static class Bootstrap
    {
        public static IStashboxContainer UseAssetManager(this IStashboxContainer container)
        {
            container.RegisterSingleton<AssetManager>();

            return container;
        }
    }
}
