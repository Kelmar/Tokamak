using System;
using System.Collections.Generic;

namespace Tokamak.Core.Drivers
{
    internal sealed class DriverRegistrar : IDriverRegistrar
    {
        /// <summary>
        /// List of meta data about drivers
        /// </summary>
        private readonly Dictionary<string, DriverInfo> m_driverMeta = new(StringComparer.InvariantCultureIgnoreCase);

        IDictionary<string, DriverInfo> IDriverRegistrar.DriverMeta => m_driverMeta;

        public DriverInfo GetDriverInfo(string id) => m_driverMeta[id];

        public void AddDriver<T>()
            where T : class, IDriver
        {
            // TODO: Find metadata about driver from type.

            string id = typeof(T).Name;

            m_driverMeta[id] = new DriverInfo
            {
                Id = id,
                DriverType = DriverType.Video,
                Name = typeof(T).Name,
                Path = string.Empty
            };
        }
    }
}