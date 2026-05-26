using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Numerics;

using Tokamak.Mathematics;

using Tokamak.Tritium.APIs;
using Tokamak.Tritium.Buffers.Formats;
using Tokamak.Tritium.Geometry;
using Tokamak.Tritium.Pipelines;
using Tokamak.Tritium.Pipelines.Shaders;
using Tokamak.Tritium.Rendering;

namespace Tokamak.Tritium.Scene
{
    public class SceneManager : IDisposable
    {
        private readonly IGraphicsLayer m_gfxLayer;

        private readonly List<IRenderPass> m_renderPasses = new();
        private readonly List<SceneObject> m_objects = new();

        private readonly IPipeline m_pipeline;
        private readonly ICommandList m_commandList;

        public SceneManager(IGraphicsLayer gfxLayer, ISceneInitializer initializer)
        {
            m_gfxLayer = gfxLayer;

            m_pipeline = initializer.GetPipeline();
            m_commandList = initializer.GetCommandList();

            m_gfxLayer.OnResize += Resize;

            Camera = new SceneCamera();

            // Force an update to our projection matrix.
            Resize(m_gfxLayer.ViewBounds);

            m_renderPasses.AddRange([
                new EnvironmentPass(m_gfxLayer, this),
                new DiffusePass(m_gfxLayer, this)
            ]);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                foreach (var o in m_objects)
                    o.Dispose();

                m_objects.Clear();

                m_commandList.Dispose();
                m_pipeline.Dispose();

                m_gfxLayer.OnResize -= Resize;
            }
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        public Matrix4x4 ProjectionMatrix => Camera.GetViewMatrix();

        public SceneCamera Camera
        {
            get;
            set => field = value ?? new SceneCamera();
        }

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
            // TODO: This logic should probably be split into Scene objects and have the SceneManager be something that "selects" a scene to render.

            m_pipeline.Activate(m_commandList);

            using var cmdScope = m_commandList.BeginScope();
            m_commandList.ClearBuffers(GlobalBuffer.ColorBuffer | GlobalBuffer.DepthBuffer | GlobalBuffer.StencilBuffer);

            m_pipeline.Uniforms.gamma = (float)Color.InverseGamma;
            m_pipeline.Uniforms.camera = Camera.Location;

            m_pipeline.Uniforms.lightColor = new Vector3(1, 1, 1);
            m_pipeline.Uniforms.lightPosition = new Vector3(10, 0, 10);

            foreach (var pass in m_renderPasses)
                pass.Render(m_pipeline, m_commandList, m_objects);
        }
    }
}
