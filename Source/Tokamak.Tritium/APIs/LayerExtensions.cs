using System;
using System.Diagnostics;

using Tokamak.Tritium.Pipelines;

namespace Tokamak.Tritium.APIs
{
    public static class LayerExtensions
    {
        [Conditional("DEBUG")]
        public static void ValidatePipelineConfig(PipelineConfig config)
        {
            if (config.InputFormat == null)
                throw new Exception("InputFormat not specified, call UseInputFormat().");
        }

        public static IPipeline CreatePipeline(this IAPILayer layer, Action<PipelineConfig> configurator)
        {
            PipelineConfig config = new PipelineConfig();

            configurator(config);

            ValidatePipelineConfig(config);

            var factory = layer.GetPipelineFactory(config);

            try
            {
                return factory.Build();
            }
            finally
            {
                if (factory is IAsyncDisposable ad)
                    ad.DisposeAsync().GetAwaiter().GetResult();
                else if (factory is IDisposable d)
                    d.Dispose();
            }
        }
    }
}
