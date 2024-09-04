using System.Collections.Generic;

namespace Tokamak.Core.Hosting
{
    /// <summary>
    /// Simple implementation for now.
    /// </summary>
    internal class GameLifetime : IGameLifetime
    {
        private readonly HashSet<ITick> m_tickers = new();

        public bool Running { get; private set; } = true;

        public void Shutdown()
        {
            Running = false;
        }

        public void AddTick(ITick tick) => m_tickers.Add(tick);

        public bool RemoveTick(ITick tick) => m_tickers.Remove(tick);

        public void Tick()
        {
            foreach (var item in m_tickers)
                item.Tick();
        }
    }
}
