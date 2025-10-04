using System;
using System.Collections.Generic;
using System.Diagnostics;

using Tokamak.Mathematics;

using Tokamak.Tritium.APIs;
using Tokamak.Tritium.Rendering;

namespace Tokamak.Tritium.Scene
{
    public class SceneManager : IDisposable
    {
        private readonly IAPILayer m_apiLayer;

        private readonly List<IRenderPass> m_renderPasses = new();
        private readonly List<SceneObject> m_objects = new();

        public SceneManager(IAPILayer apiLayer)
        {
            m_apiLayer = apiLayer;

            m_apiLayer.OnResize += Resize;

            // Force an update to our projection matrix.
            Resize(m_apiLayer.ViewBounds);

            m_renderPasses.AddRange([
                new EnvironmentPass(m_apiLayer, this),
                new DiffusePass(m_apiLayer, this)
            ]);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                foreach (var o in m_objects)
                    o.Dispose();
            }
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        public SceneCamera Camera { get; } = new SceneCamera();

        // Object management functions

        /// <summary>
        /// Gets an enumerator for all the objects within the scene.
        /// </summary>
        public IEnumerable<SceneObject> Objects => m_objects;

        public void AddObject(SceneObject obj)
        {
            Debug.Assert(obj.SceneManager == null);

            m_objects.Add(obj);
            obj.SceneManager = this;
        }

        public void RemoveObject(SceneObject obj)
        {
            Debug.Assert(obj.SceneManager == this);

            m_objects.Remove(obj);
            obj.SceneManager = null;
        }

        // Rendering functions

        public void Resize(Point size)
        {
            Camera.ViewBounds = size;
        }

        /// <summary>
        /// Render all scenes and passes.
        /// </summary>
        public void RenderAll()
        {
            foreach (var pass in m_renderPasses)
                pass.Render(m_objects);
        }
    }
}
