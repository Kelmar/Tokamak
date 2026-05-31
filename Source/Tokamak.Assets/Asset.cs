using System;
using System.Diagnostics.CodeAnalysis;
using System.Threading;

namespace Tokamak.Assets
{
    public abstract class Asset : IDisposable
    {
        private int m_disposed = 0;

        protected Asset()
        {
        }

        protected virtual void Dispose(bool disposing)
        {
        }

        public void Dispose()
        {
            if (Interlocked.Exchange(ref m_disposed, 1) == 0)
            {
                Dispose(true);
                GC.SuppressFinalize(this);
            }
        }

        [NotNull]
        public string? ID { get; internal set; }

        public bool IsDisposed => m_disposed != 0;
    }
}
