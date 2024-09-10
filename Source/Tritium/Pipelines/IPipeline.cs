using System;

using Tokamak.Tritium.APIs;

namespace Tokamak.Tritium.Pipelines
{
    public interface IPipeline : IDisposable
    {
        dynamic Uniforms { get; }

        void Activate(ICommandList commandList);
    }
}
