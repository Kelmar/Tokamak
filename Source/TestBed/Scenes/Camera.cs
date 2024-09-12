using System.Numerics;

namespace TestBed.Scenes
{
    public class Camera
    {
        private Matrix4x4 m_view = Matrix4x4.Identity;
        private bool m_dirty = true;

        private Vector3 m_location;
        private Vector3 m_lookAt;
        private Vector3 m_up = Vector3.UnitY;

        public Vector3 Location
        {
            get => m_location;
            set
            {
                m_location = value;
                m_dirty = true;
            }
        }

        public Vector3 LookAt
        {
            get => m_lookAt;
            set
            {
                m_lookAt = value;
                m_dirty = true;
            }
        }

        public Vector3 Up
        {
            get => m_up;
            set
            {
                m_up = value;
                m_dirty = true;
            }
        }

        public Matrix4x4 View
        {
            get
            {
                if (m_dirty)
                    m_view = Matrix4x4.CreateLookAt(m_location, m_lookAt, m_up);

                return m_view;
            }
        }
    }
}
