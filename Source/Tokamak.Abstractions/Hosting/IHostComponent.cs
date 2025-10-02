using System;

namespace Tokamak.Hosting.Abstractions
{
    /// <summary>
    /// Host components are items that need to run on the application's main thread.
    /// </summary>
    public interface IHostComponent : IDisposable
    {
        void Start();

        void Stop();
    }
}
