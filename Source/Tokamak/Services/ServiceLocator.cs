using System;
using System.Collections.Generic;
using System.Linq;

using Tokamak.Abstractions.Logging;
using Tokamak.Abstractions.Services;

namespace Tokamak.Services
{
    // I know people call this an anti-pattern, it was this or a full on DI framework. >_<
    public class ServiceLocator : IServiceLocator
    {
        private class ServiceInfo
        {
            public string Name { get; set; }

            public object Service { get; set; }

            public T Cast<T>() => (T)Service;
        }

        private readonly IDictionary<Type, List<ServiceInfo>> m_services = new Dictionary<Type, List<ServiceInfo>>();

        public void Register<T>(T service, string name = "")
        {
            Type t = typeof(T);

            if (!m_services.TryGetValue(t, out List<ServiceInfo> services))
            {
                services = new List<ServiceInfo>();
                m_services[t] = services;
            }

            services.Add(new ServiceInfo
            {
                Name = name,
                Service = service
            });
        }

        public void Register<T>(string name = "")
            where T : new()
        {
            Register(new T(), name);
        }

        public void Unregister<T>(T service)
        {
            Type t = typeof(T);

            if (m_services.TryGetValue(t, out List<ServiceInfo> services))
            {
                var toRemove = new List<ServiceInfo>();

                foreach (var si in services)
                {
                    if (Equals(si.Service, service))
                        toRemove.Add(si);
                }

                foreach (var si in toRemove)
                    services.Remove(si);
            }
        }

        public T Find<T>(string name = "")
        {
            Type t = typeof(T);
            ServiceInfo entry = null;

            if (m_services.TryGetValue(t, out List<ServiceInfo> services))
            {
                if (!String.IsNullOrWhiteSpace(name))
                    entry = services.FirstOrDefault(s => s.Name == name);
                else
                    entry = services.FirstOrDefault();
            }

            if (entry == null)
                throw new Exception($"Unknown service {t.Name}");

            return entry.Cast<T>();
        }

        public IEnumerable<T> GetAll<T>()
        {
            Type t = typeof(T);

            if (m_services.TryGetValue(t, out List<ServiceInfo> services))
                return services.Select(i => i.Cast<T>());

            throw new Exception($"Unknown service {t.Name}");
        }

        public ILogger GetLogger(string name = "")
        {
            var factory = Find<ILogFactory>();
            return factory.GetLogger(name);
        }

        public ILogger<T> GetLogger<T>()
        {
            var factory = Find<ILogFactory>();
            return factory.GetLogger<T>();
        }
    }
}
