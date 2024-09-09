using System;

namespace Tokamak.Tritium.APIs
{
    public interface IDeviceResource : IDisposable
    {
        void Activate();
    }
}
