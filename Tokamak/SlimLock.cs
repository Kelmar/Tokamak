using System;
using System.Threading;
using System.Threading.Tasks;

namespace Tokamak
{
    public class SlimLock : IDisposable
    {
        private readonly SemaphoreSlim m_base = new SemaphoreSlim(1, 1);

        private class Unlocker : IDisposable
        {
            private SemaphoreSlim m_sem;

            public Unlocker(SemaphoreSlim sem)
            {
                m_sem = sem;
            }

            public void Dispose()
            {
                m_sem.Release();
            }
        }

        public void Dispose()
        {
            m_base.Dispose();
        }

        public IDisposable Lock(CancellationToken cancellationToken = default)
        {
            m_base.Wait(cancellationToken);
            return new Unlocker(m_base);
        }

        public IDisposable Lock(TimeSpan timeSpan, CancellationToken cancellationToken = default)
        {
            if (m_base.Wait(timeSpan, cancellationToken))
                return new Unlocker(m_base);

            return null;
        }

        public async Task<IDisposable> LockAsync(CancellationToken cancellationToken = default)
        {
            await m_base.WaitAsync(cancellationToken);
            return new Unlocker(m_base);
        }

        public async Task<IDisposable> LockAsync(TimeSpan timeSpan, CancellationToken cancellationToken= default)
        {
            if (await m_base.WaitAsync(timeSpan, cancellationToken))
                return new Unlocker(m_base);

            return null;
        }
    }
}
