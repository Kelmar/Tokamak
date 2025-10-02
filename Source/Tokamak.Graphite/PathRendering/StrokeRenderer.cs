using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Tokamak.Graphite.PathRendering
{
    /*
     * To help understand what the math is doing here it, is best to think
     * of the line as being more like a vector with a start and a direction
     * pointing to the end.  When we consider this, the line then has a left
     * side and a right side which is based on it's direction:
     * 
     *         left
     * start --------> end
     *         right
     * 
     * It is important to note that this notion of left and right physically
     * changes on screen, depending on the relation of start to end.
     * 
     * For example:
     * 
     *        right
     * end <--------- start
     *        left
     * 
     * This can help us determine a few things.  For lines with a thickness,
     * we can consider the left side the "outside" of our path, and the right
     * the "inside" of our path, provided our path is drawn with a clockwise
     * winding.
     * 
     * This also helps us figure out which way a line is turning when we hit
     * a new point and it goes off into a new direction.
     * 
     * We do this by taking the cross product of the two vectors, or rather
     * we compute the Z component of a cross product of our 2D vectors as
     * if they are 3D vectors with their Z component set to zero, which
     * effectively will always return zero for the X and Y in the resulting
     * cross product.
     */

    internal class StrokeRenderer
    {
        private readonly Stroke m_stroke;

        private readonly List<PointInfo> m_points = new();

        public StrokeRenderer(Stroke stroke)
        {
            m_stroke = stroke;
        }

        public void Render()
        {
            if (m_stroke.Points.Count <= 1)
                return; // Nothing to render
        }
    }
}
