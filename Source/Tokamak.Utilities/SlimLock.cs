using System;
using System.Threading;
using System.Threading.Tasks;

namespace Tokamak.Utilities
{
    /// <summary>
    /// Wrapper around SemaphoreSlim() to provide a simple entry lock.
    /// </summary>
    public sealed class SlimLock : IDisposable
    {
        public enum Error
        {
            NoError = 0,
            Timeout = 1,
        }

        private readonly SemaphoreSlim m_base = new SemaphoreSlim(1, 1);

        public void Dispose()
        {
            m_base.Dispose();
            GC.SuppressFinalize(this);
        }

        private DisposeAction Unlocker() =>
            new DisposeAction(() => m_base.Release());

        public IDisposable Lock(CancellationToken cancellationToken = default)
        {
            m_base.Wait(cancellationToken);
            return Unlocker();
        }

        public Result<IDisposable, Error> Lock(TimeSpan timeSpan, CancellationToken cancellationToken = default)
        {
            if (m_base.Wait(timeSpan, cancellationToken))
                return Unlocker();

            return Error.Timeout;
        }

        public async Task<IDisposable> LockAsync(CancellationToken cancellationToken = default)
        {
            await m_base.WaitAsync(cancellationToken);
            return Unlocker();
        }

        public async Task<Result<IDisposable, Error>> LockAsync(TimeSpan timeSpan, CancellationToken cancellationToken = default)
        {
            if (await m_base.WaitAsync(timeSpan, cancellationToken))
                return Unlocker();

            return Error.Timeout;
        }
    }
}
