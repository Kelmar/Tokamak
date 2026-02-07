using System.Numerics;

namespace Tokamak.Graphite.PathRendering
{

    /// <summary>
    /// Computed line segment along a path.
    /// </summary>
    internal class PathSegment
    {
        private Vector2 m_start;
        private Vector2 m_end;

        public PathSegment(Vector2 start, Vector2 end)
        {
            m_start = start;
            m_end = end;

            ComputeDirection();
        }

        private void ComputeDirection()
        {
            Direction = m_end - m_start;

            float length = Direction.Length();

            if (length != 0) // Avoid NaN while normalizing
                Direction /= length;
        }

        /// <summary>
        /// The starting coordinate for the segment.
        /// </summary>
        public Vector2 Start
        {
            get => m_start;
            set
            {
                m_start = value;
                ComputeDirection();
            }
        }

        /// <summary>
        /// The ending coordinate for the segment.
        /// </summary>
        public Vector2 End
        {
            get => m_end;
            set
            {
                m_end = value;
                ComputeDirection();
            }
        }

        /// <summary>
        /// Gets the direction vector of the segment.
        /// </summary>
        /// <remarks>
        /// Value is already normalized.
        /// </remarks>
        public Vector2 Direction { get; private set; }

        public override string ToString() => $"{Start} -> {End}";
    }
}
