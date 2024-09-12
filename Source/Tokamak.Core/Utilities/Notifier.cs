using System;
using System.Collections.Generic;

namespace Tokamak.Core.Utilities
{
    /// <summary>
    /// Class for sending a notice out to a group of watchers.
    /// </summary>
    public class Notifier : INotifier
    {
        private sealed class Remover : IDisposable
        {
            private readonly ISet<Action> m_watchers;
            private readonly Action m_watcher;

            public Remover(ISet<Action> watchers, Action watcher)
            {
                m_watchers = watchers;
                m_watcher = watcher;
            }

            public void Dispose()
            {
                m_watchers.Remove(m_watcher);
            }
        }

        private HashSet<Action> m_watchers = new();

        /// <summary>
        /// Gets if the notifier is currently in a suspended state.
        /// </summary>
        /// <remarks>
        /// A suspended notifier will not send out notifications until it is resumed.
        /// </remarks>
        public bool IsSuspended { get; private set; } = false;

        /// <summary>
        /// Subscribe to a notifier.
        /// </summary>
        /// <param name="watcher">The action to call when the notice is being sent.</param>
        /// <returns>An IDisposable that will unsubscribe the watcher when it is disposed of.</returns>
        public IDisposable Subscribe(Action watcher)
        {
            m_watchers.Add(watcher);
            return new Remover(m_watchers, watcher);
        }

        /// <summary>
        /// Suspends the notifier until a call to resume.
        /// </summary>
        public void Suspend()
        {
            IsSuspended = true;
        }

        /// <summary>
        /// Resumes the notifier.
        /// </summary>
        public void Resume()
        {
            IsSuspended = false;
        }

        /// <summary>
        /// Notify all of the watchers of the event.
        /// </summary>
        public void Raise()
        {
            if (IsSuspended)
                return; // Do not send notices when we are suspended.

            foreach (var watch in m_watchers)
                watch();
        }
    }

    /// <summary>
    /// Class for sending a notice out to a group of watchers.
    /// </summary>
    /// <typeparam name="T">Value type to transmit to the watchers.</typeparam>
    public class Notifier<T> : INotifier<T>
    {
        private sealed class Remover : IDisposable
        {
            private readonly ISet<Action<T>> m_watchers;
            private readonly Action<T> m_watcher;

            public Remover(ISet<Action<T>> watchers, Action<T> watcher)
            {
                m_watchers = watchers;
                m_watcher = watcher;
            }

            public void Dispose()
            {
                m_watchers.Remove(m_watcher);
            }
        }

        private HashSet<Action<T>> m_watchers = new();

        /// <summary>
        /// Gets if the notifier is currently in a suspended state.
        /// </summary>
        /// <remarks>
        /// A suspended notifier will not send out notifications until it is resumed.
        /// </remarks>
        public bool IsSuspended { get; private set; } = false;

        /// <summary>
        /// Subscribe to a notifier.
        /// </summary>
        /// <param name="watcher">The action to call when the notice is being sent.</param>
        /// <returns>An IDisposable that will unsubscribe the watcher when it is disposed of.</returns>
        public IDisposable Subscribe(Action<T> watcher)
        {
            m_watchers.Add(watcher);
            return new Remover(m_watchers, watcher);
        }

        /// <summary>
        /// Suspends the notifier until a call to resume.
        /// </summary>
        public void Suspend()
        {
            IsSuspended = true;
        }

        /// <summary>
        /// Resumes the notifier.
        /// </summary>
        public void Resume()
        {
            IsSuspended = false;
        }

        /// <summary>
        /// Notify all of the watchers of the event.
        /// </summary>
        /// <param name="value"></param>
        public void Raise(T value)
        {
            if (IsSuspended)
                return; // Do not send notices when we are suspended.

            foreach (var watch in m_watchers)
                watch(value);
        }
    }
}
