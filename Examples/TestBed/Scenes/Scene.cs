using System;
using System.Collections.Generic;
using System.IO;
using System.Numerics;

using Tokamak.Mathematics;

using Tokamak.Tritium.APIs;
using Tokamak.Tritium.Buffers.Formats;
using Tokamak.Tritium.Geometry;
using Tokamak.Tritium.Pipelines;
using Tokamak.Tritium.Pipelines.Shaders;
using Tokamak.Tritium.Scene;

namespace TestBed.Scenes
{
    public class Scene : IDisposable//, IRenderable
    {
        private readonly IGraphicsLayer m_apiLayer;

        private readonly IPipeline m_pipeline;
        private readonly ICommandList m_commandList;

        private readonly List<SceneObject> m_objects = new List<SceneObject>();

        private SceneCamera m_camera = new SceneCamera();

        public Scene(IGraphicsLayer apiLayer)
        {
            m_apiLayer = apiLayer;

            m_apiLayer.OnResize += Resize;

            string vertexShader = File.ReadAllText("shaders/basic.vert");
            string fragmentShader = File.ReadAllText("shaders/basic.frag");

            m_pipeline = m_apiLayer.CreatePipeline(cfg =>
            {
                cfg.UseInputFormat<VectorFormatPNCT>();

                cfg.UseCulling(CullMode.None);
                cfg.EnableDepthTest(true);
                cfg.UsePrimitive(PrimitiveType.TriangleList);

                cfg.AddShaderCode(ShaderType.Vertex, vertexShader);
                cfg.AddShaderCode(ShaderType.Fragment, fragmentShader);
            });

            m_commandList = m_apiLayer.CreateCommandList();

            Resize(m_apiLayer.ViewBounds);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                foreach (var obj in m_objects)
                    obj.Dispose();

                m_commandList.Dispose();
                m_pipeline.Dispose();

                m_apiLayer.OnResize -= Resize;
            }
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        public Matrix4x4 Projection { get; private set; }

        public SceneCamera Camera
        {
            get => m_camera;
            set => m_camera = value ?? new SceneCamera();
        }

        public void AddObject(SceneObject obj)
        {
            m_objects.Add(obj);
        }

        public void RemoveObject(SceneObject obj)
        {
            m_objects.Remove(obj);
        }

        public void Resize(Point size)
        {
            Camera.ViewBounds = new Vector2(size.X, size.Y);
            Projection = Camera.GetViewMatrix();
        }

        public void Render()
        {
            m_pipeline.Activate(m_commandList);

            using var cmdScope = m_commandList.BeginScope();
            m_commandList.ClearBuffers(GlobalBuffer.ColorBuffer | GlobalBuffer.DepthBuffer | GlobalBuffer.StencilBuffer);

            m_pipeline.Uniforms.projection = Projection;
            m_pipeline.Uniforms.view = m_camera.GetProjectionMatrix();
            m_pipeline.Uniforms.gamma = (float)Color.InverseGamma;
            m_pipeline.Uniforms.camera = m_camera.Location;

            m_pipeline.Uniforms.lightColor = new Vector3(1, 1, 1);
            m_pipeline.Uniforms.lightPosition = new Vector3(10, 0, 10);

            foreach (var obj in m_objects)
            {
                m_pipeline.Uniforms.model = obj.Model;
                obj.Render(m_commandList);
            }
        }
    }
}
