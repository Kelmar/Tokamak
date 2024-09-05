using Stashbox;

using System;
using System.Collections.Generic;
using System.Linq;

using Tokamak.Core.Config;
using Tokamak.Core.Logging;

using Tokamak.Tritium.APIs.NullRender;

namespace Tokamak.Tritium.APIs
{
    internal class APILoader
    {
        private readonly ILogger m_log;
        private readonly IDependencyResolver m_resolver;
        private readonly IDictionary<string, IAPIDescriptor> m_descriptors;
        private readonly TritiumConfig m_config;

        public APILoader(
            ILogger<APILoader> log,
            IDependencyResolver resolver,
            IOptions<TritiumConfig> config,
            IList<IAPIDescriptor> descriptors)
        {
            m_log = log;
            m_resolver = resolver;
            m_descriptors = descriptors.ToDictionary(a => a.ID, a => a, StringComparer.InvariantCultureIgnoreCase);
            m_config = config.Value;
        }

        public IAPIDescriptor SelectAPI()
        {
            IAPIDescriptor rval = null;

            if (m_config.Headless)
            {
                rval = m_resolver.Activate<NullAPI>();
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
                    throw new Exception($"Unable to find a suitable graphics API to use!");
            }

            m_log.Info("Using Graphics API: {0}", rval.Name);

            return rval;
        }
    }
}
