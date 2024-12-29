using Tokamak.Tritium.APIs;

namespace Tokamak.Tritium.Pipelines
{
    public interface IPipeline : IDeviceResource
    {
        dynamic Uniforms { get; }

        void Activate(ICommandList commandList);
    }
}
