using System;

namespace Tokamak
{
    public interface IDeviceResource : IDisposable
    {
        void Activate();
    }
}
