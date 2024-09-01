using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;

using Stashbox;

using Tokamak.Core.Config;
using Tokamak.Core.Logging;

namespace Tokamak.Core.Drivers
{
    internal sealed class DriverLoader : IDisposable, IDriverLoader
    {
        private enum DriverState
        {
            Errored = -1000,

            NotLoaded = 0,
            PreLoaded = 1,
            Loaded = 2,
            Unloaded = 3
        }

        private class DriverDetails
        {
            /// <summary>
            /// Driver meta data
            /// </summary>
            public DriverInfo Info { get; set; }

            /// <summary>
            /// Convenience method for getting driver scope.
            /// </summary>
            public string DriverId => Info.Id;

            /// <summary>
            /// Assembly that was loaded (if there was one)
            /// </summary>
            public Assembly Assembly { get; set; } = null;

            /// <summary>
            /// The loaded driver
            /// </summary>
            public IDriver Driver { get; set; } = null;

            /// <summary>
            /// The state of this particular driver
            /// </summary>
            public DriverState State { get; set; } = DriverState.NotLoaded;

            /// <summary>
            /// The last exception that caused an error
            /// </summary>
            public Exception LastError { get; set; } = null;
        }

        private readonly IGameHost m_host;
        private readonly ILogger m_log;
        private readonly IDriverRegistrar m_driverRegistrar;
        private readonly DriverConfig m_config;

        private readonly List<DriverDetails> m_drivers = new();

        public DriverLoader(IGameHost host)
        {
            m_host = host;
            m_log = host.Services.Resolve<ILogger>();
            m_driverRegistrar = host.Services.Resolve<IDriverRegistrar>();
            m_config = host.Services.Resolve<IOptions<DriverConfig>>().Value;
        }

        public void Dispose()
        {
            foreach (var details in m_drivers)
                details.Driver?.Dispose();
        }

        private void Try(DriverDetails details, Action<DriverDetails> fn, DriverState nextState)
        {
            using var section = m_log.BeginScope(new { details.DriverId });

            try
            {
                fn(details);
                details.State = nextState;
            }
            catch (Exception ex)
            {
                details.State = DriverState.Errored;
                details.LastError = ex;
            }
        }

        /// <summary>
        /// Called before the main window is created to provide any 
        /// pre initialization information about the window's creation.
        /// </summary>
        public void Preload()
        {
            HashSet<string> toLoad = [ m_config.Video, m_config.Audio ];

            foreach (var info in m_driverRegistrar.DriverMeta.Values)
            {
                if (!toLoad.Contains(info.Id))
                    continue; // Do not load this driver

                using var section = m_log.BeginScope(new { DriverId = info.Id });

                m_log.Info("Preloading driver: {name}", info.Id, info.Name);

                var details = new DriverDetails
                {
                    Info = info
                };

                m_drivers.Add(details);

                Try(details, PreloadDriver, DriverState.PreLoaded);
            }
        }

        /// <summary>
        /// Called after the main window is created to allow the driver
        /// to fully load itself against the newly created OS resource.
        /// </summary>
        public void Load()
        {
            var preloaded = m_drivers.Where(d => d.State == DriverState.PreLoaded);

            foreach (var details in preloaded)
                Try(details, d => d.Driver.Load(), DriverState.Loaded);
        }

        /// <summary>
        /// Called just before the destruction of the main window so
        /// the driver can perform any clean up tasks before hand.
        /// </summary>
        public void Unload()
        {
            var loaded = m_drivers.Where(d => d.State == DriverState.Loaded);

            foreach (var details in loaded)
                Try(details, d => d.Driver.Unload(), DriverState.Unloaded);
        }

        private void PreloadDriver(DriverDetails details)
        {
            if (m_host.Services.CanResolve<IDriver>(details.Info.DriverType))
                throw new Exception($"BUG: {details.Info.DriverType} driver already preloaded!");

            if (!String.IsNullOrWhiteSpace(details.Info.Path))
            {
                // Try to load assembly with given path name.
                details.Assembly = Assembly.LoadFile(details.Info.Path);
            }

            /*
            Type t = typeof(Driver);
            details.Driver = (IDriver)m_host.Services.Activate(t);

            details.Driver.Preload();

            m_host.Services.PutInstanceInScope(details.Driver, withoutDisposalTracking: true, details.Info.DriverType);
            */
        }
    }
}
