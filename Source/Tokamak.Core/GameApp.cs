using System;

namespace Tokamak.Core
{
    public class GameApp : IGameApp
    {
        public GameApp(IGameHost host)
        {
            Host = host;
        }

        protected virtual void Dispose(bool disposing)
        {
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        public IGameHost Host { get; }

        virtual public void OnLoad()
        {
        }

        virtual public void OnShutdown()
        {
        }

        virtual public void OnUpdate(double timeDelta)
        {
        }

        virtual public void OnRender(double timeDelta)
        {
        }
    }
}
