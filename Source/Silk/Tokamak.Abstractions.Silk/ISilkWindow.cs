using Silk.NET.Windowing;

namespace Tokamak.Abstractions.Silk
{
    public interface ISilkWindow
    {
        IView View { get; }

        IWindow? Window { get; }
    }
}
