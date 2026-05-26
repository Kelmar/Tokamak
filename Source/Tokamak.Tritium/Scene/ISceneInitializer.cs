using Tokamak.Tritium.APIs;
using Tokamak.Tritium.Pipelines;

namespace Tokamak.Tritium.Scene
{
    public interface ISceneInitializer
    {
        IPipeline GetPipeline();

        ICommandList GetCommandList();
    }
}
