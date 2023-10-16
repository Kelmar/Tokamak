using System.Collections.Generic;

using Tokamak.Logging;

namespace Tokamak.Services
{
    // I know people call this an anti-pattern, it was this or a full on DI framework. >_<
    public interface IServiceLocator
    {
        void Register<T>(T service, string name = "");

        void Register<T>(string name = "")
            where T : new();

        T Find<T>(string name = "");

        IEnumerable<T> GetAll<T>();

        ILogger GetLogger(string name = "");

        ILogger<T> GetLogger<T>();
    }
}
