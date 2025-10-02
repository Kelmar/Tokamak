using System.Collections.Generic;
using System.Linq;

using Tokamak.Hosting.Abstractions;

namespace Tokamak.Hosting.Implementation
{
    /// <summary>
    /// Simple implementation for now.
    /// </summary>
    internal class GameLifetime : IGameLifetime
    {
        private class TickInfo
        {
            public required TickPriority Priority { get; set; }

            public required ITick Ticker { get; set; }
        }

        private readonly Dictionary<ITick, TickInfo> m_tickers = new();

        private readonly Dictionary<TickPriority, List<ITick>> m_priorityLists = new();

        private readonly TickPriority[] m_priorities;

        public GameLifetime()
        {
            var t = typeof(TickPriority);

            var values = (TickPriority[])t.GetEnumValues();
            m_priorities = values.OrderDescending().ToArray();

            foreach (var value in m_priorities)
                m_priorityLists[value] = new();
        }

        public bool Running { get; private set; } = true;

        public void Shutdown()
        {
            Running = false;
        }

        public void AddTick(ITick tick, TickPriority priority = TickPriority.Normal)
        {
            if (m_tickers.TryGetValue(tick, out TickInfo? info))
            {
                if (info.Priority == priority)
                    return; // No change;

                m_priorityLists[priority].Remove(tick);
                info.Priority = priority;
            }
            else
            {
                info = new TickInfo
                {
                    Priority = priority,
                    Ticker = tick
                };

                m_tickers[tick] = info;
            }

            m_priorityLists[priority].Add(info.Ticker);
        }

        public bool RemoveTick(ITick tick)
        {
            if (m_tickers.TryGetValue(tick, out TickInfo? info))
            {
                m_tickers.Remove(tick);
                m_priorityLists[info.Priority].Remove(tick);
                return true;
            }

            return false;
        }

        public void Tick()
        {
            foreach (var value in m_priorities)
            {
                foreach (var item in m_priorityLists[value])
                    item.Tick();
            }
        }
    }
}
