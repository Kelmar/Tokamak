using System;

namespace Tokamak
{
    public interface IPipelineFactory : IDisposable
    {
        IPipeline Build();
    }
}
