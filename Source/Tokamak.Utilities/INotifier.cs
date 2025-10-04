using System;

namespace Tokamak.Utilities
{
    /// <summary>
    /// Class for sending a notice out to a group of watchers.
    /// </summary>
    public interface INotifier
    {
        /// <summary>
        /// Gets if the notifier is currently in a suspended state.
        /// </summary>
        /// <remarks>
        /// A suspended notifier will not send out notifications until it is resumed.
        /// </remarks>
        bool IsSuspended { get; }

        /// <summary>
        /// Subscribe to a notifier.
        /// </summary>
        /// <param name="watcher">The action to call when the notice is being sent.</param>
        /// <returns>An IDisposable that will unsubscribe the watcher when it is disposed of.</returns>
        IDisposable Subscribe(Action watcher);

        /// <summary>
        /// Suspends the notifier until a call to Resume().
        /// </summary>
        void Suspend();

        /// <summary>
        /// Resumes the sending of notices.
        /// </summary>
        void Resume();

        /// <summary>
        /// Sends out a notice to all of the subscribed watchers provided the notifier isn't suspended.
        /// </summary>
        void Raise();
    }

    /// <summary>
    /// Class for sending a notice out to a group of watchers.
    /// </summary>
    /// <typeparam name="T">Value type to transmit to the watchers.</typeparam>
    public interface INotifier<T>
    {
        /// <summary>
        /// Gets if the notifier is currently in a suspended state.
        /// </summary>
        /// <remarks>
        /// A suspended notifier will not send out notifications until it is resumed.
        /// </remarks>
        bool IsSuspended { get; }

        /// <summary>
        /// Subscribe to a notifier.
        /// </summary>
        /// <param name="watcher">The action to call when the notice is being sent.</param>
        /// <returns>An IDisposable that will unsubscribe the watcher when it is disposed of.</returns>
        IDisposable Subscribe(Action<T> watcher);

        /// <summary>
        /// Suspends the notifier until a call to Resume().
        /// </summary>
        void Suspend();

        /// <summary>
        /// Resumes the sending of notices.
        /// </summary>
        void Resume();

        /// <summary>
        /// Sends out a notice to all of the subscribed watchers provided the notifier isn't suspended.
        /// </summary>
        /// <param name="value">The value to transmit to the subscribers.</param>
        void Raise(T value);
    }
}
