using System.Threading;

namespace Tokamak.Utilities
{
    /// <summary>
    /// Provides a thread safe reference counter that can be incremented or decremented.
    /// </summary>
    /// <remarks>
    /// This is similar to a <see cref="System.Runtime.CompilerServices.StrongBox"/>&lt;int&gt; in that it
    /// provides a boxed integer that can be reference passed and modified.  The difference is that it is
    /// gated behind thread safe <see cref="System.Threading.Interlock"/>.Increment/Decrement calls, so that
    /// access to the underlying value is maintained in a consistent thread safe way.
    /// </remarks>
    public class ReferenceCounter
    {
        private int m_value;

        /// <summary>
        /// Initializes the reference counter.
        /// </summary>
        /// <param name="value">The initial value to set the count to.  Defaults to 1</param>
        public ReferenceCounter(int value = 1)
        {
            m_value = value;
        }

        public int Value => m_value;

        /// <summary>
        /// Increments the reference counter.
        /// </summary>
        /// <returns>The value of the reference counter immediately after it was incremented.</returns>
        public int Increment()
        {
            return Interlocked.Increment(ref m_value);
        }

        /// <summary>
        /// Decrements the reference counter.
        /// </summary>
        /// <returns>The value of the reference counter immediately after it was decremented.</returns>
        public int Decrement()
        {
            return Interlocked.Decrement(ref m_value);
        }
    }
}
