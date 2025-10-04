using System;

namespace Tokamak.Utilities
{
    /// <summary>
    /// Utility that will call the supplied action when it is disposed.
    /// </summary>
    /// <remarks>
    /// This is similar in behavior to a finally keyword, except that it
    /// can be returned to a caller so that it may clean up a resource
    /// when it is done with it.
    /// 
    /// This is mostly useful for when writing a whole new class to
    /// effectively execute a single method would normally be overkill.
    /// </remarks>
    public class DisposeAction : IDisposable
    {
        private Action m_action;

        public DisposeAction(Action a)
        {
            m_action = a;
        }

        public void Dispose()
        {
            m_action();
            GC.SuppressFinalize(this);
        }
    }

    /// <inheritdoc />
    public class DisposeAction<T> : DisposeAction
    {
        public DisposeAction(Action<T> a, T val)
            : base(() => a(val))
        {
        }
    }

    /// <inheritdoc />
    public class DisposeAction<T1, T2> : DisposeAction
    {
        public DisposeAction(Action<T1, T2> a, T1 a1, T2 a2)
            : base(() => a(a1, a2))
        {
        }
    }
}
