using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

using Stashbox;

using Tokamak.Abstractions.Logging;
using Tokamak.Abstractions.Config;

namespace Tokamak.Core.Hosting
{
    /// <summary>
    /// GameHost implementation.
    /// </summary>
    [LogName("GameHost")]
    public class GameHost : IGameHost
    {
        private readonly IStashboxContainer m_container;
        private readonly IDependencyResolver m_scope;

        private readonly List<IBackgroundService> m_services = new();
        private readonly List<IHostComponent> m_components = new();

        private readonly GameLifetime m_gameLifetime = new();

        private readonly Lazy<IGameApp> m_app;

        public GameHost(IGameHostBuilder builder)
        {
            m_container = builder.Container;

            // Allow things to resolve us, but don't dispose, we're managing the lifetime of the container itself!
            m_container.RegisterInstance<IGameHost>(this, withoutDisposalTracking: true);
            m_container.RegisterInstance<IGameLifetime>(m_gameLifetime, withoutDisposalTracking: true);

            m_scope = m_container.BeginScope("Main Scope");

            Configuration = m_scope.Resolve<IConfiguration>();

            Log = m_scope.Resolve<ILogger<GameHost>>();

            Console.CancelKeyPress += AbortApp;

            // Resolve any dependencies only when we really need to.
            m_app = new Lazy<IGameApp>(() => m_scope.Resolve<IGameApp>());
        }

        virtual protected void Dispose(bool disposing)
        {
            if (disposing)
            {
                Console.CancelKeyPress -= AbortApp;

                m_scope.Dispose();
                m_container.Dispose();
            }
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        public IDependencyResolver Services => m_scope;

        public IConfiguration Configuration { get; }

        public IGameApp App => m_app.Value;

        protected ILogger Log { get; }

        public void SendBackgroundCommand(Func<IBackgroundService, Task> method)
        {
            var tasks = m_services.Select(method);

            Task.WhenAll(tasks)
                .ConfigureAwait(false)
                .GetAwaiter()
                .GetResult();
        }

        private void AbortApp(object? _, ConsoleCancelEventArgs e)
        {
            if (m_gameLifetime.Running)
            {
                // Try for a graceful shutdown first.

                Log.Warn("CTRL+C pressed, terminating application....");
                m_gameLifetime.Shutdown();
                e.Cancel = true;
            }
            else
            {
                Log.Error("CTRL+C pressed twice, terminating!");
                e.Cancel = false;
                Environment.Exit(-1);
            }
        }

        protected virtual void StartBackground()
        {
            Log.Info("Starting background services...");
            m_services.AddRange(m_scope.ResolveAll<IBackgroundService>());

            SendBackgroundCommand(s => s.StartAsync());
        }

        protected virtual void StopBackground()
        {
            using var cts = new CancellationTokenSource(TimeSpan.FromSeconds(30));

            Log.Info("Stopping background services...");
            SendBackgroundCommand(s => s.StopAsync(cts.Token));

            if (cts.IsCancellationRequested)
                Log.Info("Background services timed out on shutdown!");
        }

        protected virtual void StartComponents()
        {
            Log.Info("Starting main thread items...");

            m_components.AddRange(m_scope.ResolveAll<IHostComponent>());

            m_components.ForEach(c => c.Start());
        }

        protected virtual void StopComponents()
        {
            Log.Info("Stopping main thread items...");

            m_components.ForEach(c => c.Stop());
        }

        public virtual void Start()
        {
            StartBackground();
            StartComponents();

            // The App.OnLoad() gets called by the API Layer when it creates the main window.
        }

        public virtual void MainLoop()
        {
            Log.Info("MainLoop() now running.  (Press CTRL+C to terminate)");

            var timer = Stopwatch.StartNew();

            while (m_gameLifetime.Running)
            {
                m_gameLifetime.Tick();

                double delta = timer.Elapsed.TotalSeconds; // Get delta for the frame.
                timer.Restart();

                App.OnUpdate(delta);
            }
        }

        public virtual void Stop()
        {
            App.OnShutdown();

            StopComponents();
            StopBackground();
        }
    }
}
