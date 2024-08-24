using Tokamak.Core.Services;

namespace Tokamak.Core
{
    public class GameApplication
    {
        public GameApplication()
        {
            Services = new ServiceLocator();
        }

        public IServiceLocator Services { get; }

        public void Run()
        {
        }
    }
}
