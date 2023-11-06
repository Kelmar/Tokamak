using System;

namespace Tokamak
{
    public interface IPipeline : IDisposable
    {
        void Activate();
    }
}
