using System;
using System.Collections.Generic;

using Tokamak.Core.Logging;

namespace Tokamak.Core.Services
{
    // I know people call this an anti-pattern, it was this or a full on DI framework. >_<
    public interface IServiceLocator : IDisposable
    {
        void Register<T>(T service, string name = "");

        void Register<T>(string name = "")
            where T : new();

        void Unregister<T>(T service);

        T Find<T>(string name = "");

        IEnumerable<T> GetAll<T>();

        ILogger GetLogger(string name = "");

        ILogger<T> GetLogger<T>();
    }
}
