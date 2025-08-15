using Silk.NET.Maths;

namespace Tokamak.Mathematics.Silk
{
    public static class RectX
    {
        public static Rect ToRect(this in Rectangle<int> r)
            => new Rect(r.Origin.ToPoint(), r.Size.ToPoint());

        public static Rectangle<int> ToSilkRect(this in Rect r)
            => new Rectangle<int>(r.Location.ToSilkVector(), r.Size.ToSilkVector());
    }
}
