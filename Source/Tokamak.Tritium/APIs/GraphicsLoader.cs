using System;
using System.Collections.Generic;
using System.Linq;

using Tokamak.Config.Abstractions;
using Tokamak.Logging.Abstractions;

using Tokamak.Tritium.APIs.NullRender;

namespace Tokamak.Tritium.APIs
{
    [LogName("GraphicsLoader")]
    internal class GraphicsLoader
    {
        private readonly ILogger m_log;
        private readonly TritiumConfig m_config;
        private readonly IDictionary<string, IGraphicsDescriptor> m_descriptors;

        public GraphicsLoader(
            ILogger<GraphicsLoader> log,
            IOptions<TritiumConfig> config,
            IList<IGraphicsDescriptor> descriptors)
        {
            m_log = log;
            m_config = config.Value;
            m_descriptors = descriptors.ToDictionary(a => a.ID, a => a, StringComparer.InvariantCultureIgnoreCase);
        }

        private IGraphicsDescriptor SelectAPI()
        {
            IGraphicsDescriptor? rval = null;

            if (m_config.Headless)
            {
                rval = new NullAPI();
                m_log.Debug("Starting in headless mode, using {0}.", rval.Name);
            }
            else if (!string.IsNullOrWhiteSpace(m_config.API))
            {
                if (!m_descriptors.TryGetValue(m_config.API, out rval))
                    m_log.Error("Invalid graphics API '{0}' selected, trying default.", m_config.API);
            }
            else
            {
                m_log.Debug("No graphics API selected, using default.");
            }

            if (rval == null)
            {
                // Try for the most desirable API available.
                rval = m_descriptors.Values
                    .Where(g => g.SupportLevel > SupportLevel.NoSupport)
                    .OrderByDescending(g => (int)g.SupportLevel)
                    .FirstOrDefault();

                if (rval == null)
                    throw new NotSupportedException("Unable to find a suitable graphics API to use!");
            }

            m_log.Info("Using Graphics API: {0}", rval.Name);

            return rval;
        }

        public IGraphicsLayer Build()
        {
            var descriptor = SelectAPI();
            return descriptor.Build();
        }
    }
}
