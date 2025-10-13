using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;

using Tokamak.Graphite.PathRendering;

namespace Tokamak.Graphite
{
    internal class Path
    {
        internal List<Stroke> m_strokes = new();

        private Stroke m_current = new();

        public Path()
        {
            m_strokes.Add(m_current);
        }

        /// <summary>
        /// Add in first move to point if needed.
        /// </summary>
        /// <remarks>
        /// Add a staring point at the zero vector if there
        /// are no other points yet added to the Points list.
        /// </remarks>
        private void AddFirstMove()
        {
            if (m_current.Points.Count > 0 && !m_current.Closed)
                return;

            if (m_current.Closed)
            {
                Stroke l = m_current;

                m_current = new();
                m_current.Points.Add(l.Points.Last());
                m_strokes.Add(m_current);
            }
            else
                m_current.Points.Add(Vector2.Zero);
        }

        public void MoveTo(float x, float y)
            => MoveTo(new Vector2(x, y));

        public void MoveTo(in Vector2 v)
        {
            if (m_current.Points.Count > 1)
            {
                m_current = new();
                m_strokes.Add(m_current);
            }

            if (m_current.Points.Count == 0)
                m_current.Points.Add(v);
            else
                m_current.Points[0] = v;
        }

        public void LineTo(float x, float y)
            => LineTo(new Vector2(x, y));

        public void LineTo(in Vector2 v)
        {
            AddFirstMove();

            m_current.Points.Add(v);
            m_current.Actions.Add(PathAction.Line);
        }

        public void Close()
        {
            // Only close if we have enough points to make a meaningful loop.
            if (m_current.Points.Count < 3)
                return;

            m_current.Closed = true;
        }
    }
}
