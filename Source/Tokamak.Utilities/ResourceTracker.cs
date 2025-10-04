using System;
using System.Collections.Generic;
using System.Linq;

namespace Tokamak.Utilities
{
    /// <summary>
    /// Used for tracking a list of items that need to be disposed of.
    /// </summary>
    /// <remarks>
    /// This is helpful when you have several items created in a loop that need to be tracked
    /// outside of the loop's scope as well.
    /// </remarks>
    public sealed class ResourceTracker : IDisposable
    {
        private readonly IList<IDisposable> m_items = new List<IDisposable>();

        public void Dispose()
        {
            m_items.Reverse(); // Clean up in reverse of the order they were inserted in.

            foreach (var item in m_items)
                item.Dispose();
        }

        public void Add(IDisposable item)
        {
            m_items.Add(item);
        }
    }
}
