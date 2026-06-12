using Stashbox;

using Tokamak.Abstractions.Input;

namespace Tokamak.Silk.Input
{
    public static class Bootstrap
    {
        public static IStashboxContainer UseSilkInput(this IStashboxContainer container)
        {
            container.RegisterSingleton<IInputManager, SilkInputManager>();

            return container;
        }
    }
}
