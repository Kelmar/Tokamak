namespace Tokamak.Hosting.Abstractions
{
    public interface ITick
    {
        void Tick();
    }

    /// <summary>
    /// Used for grouping ticks so they run in a rough order.
    /// </summary>
    public enum TickPriority
    {
        Lowest  = -2,
        Low     = -1,
        Normal  = 0,
        High    = 1,
        Highest = 2
    }
}
