﻿using System;
using System.Collections.Generic;
using System.Numerics;

using Tokamak.Mathematics;

using Tokamak.Tritium.APIs;
using Tokamak.Tritium.Buffers.Formats;
using Tokamak.Tritium.Pipelines;
using Tokamak.Tritium.Pipelines.Shaders;

namespace TestBed.Scenes
{
    public class Scene : IDisposable//, IRenderable
    {
        public const string VERTEX = @"#version 450

uniform mat4 projection;
uniform mat4 view;
uniform mat4 model;

layout(location = 0) in vec3 Point;
layout(location = 1) in vec4 Color;
layout(location = 2) in vec2 TexCoord;

layout(location = 0) out vec4 fsin_Color;
layout(location = 1) out vec2 fsin_TexCoord;

void main()
{
    gl_Position = vec4(Point, 1.0) * model * view * projection;
    fsin_Color = Color;
    fsin_TexCoord = TexCoord;
}
";


        public const string FRAGMENT = @"#version 450

layout(location = 0) in vec4 fsin_Color;
layout(location = 1) in vec2 fsin_TexCoord;

layout(location = 0) out vec4 fsout_Color;

uniform int is8Bit;
uniform sampler2D texture0;

void main()
{
    vec4 tx = texture(texture0, fsin_TexCoord);

    fsout_Color = is8Bit != 0 ? vec4(fsin_Color.rgb, fsin_Color.a * tx.r) : tx * fsin_Color;
}
";
        private readonly IAPILayer m_apiLayer;

        private readonly IPipeline m_pipeline;
        private readonly ICommandList m_commandList;

        private readonly List<SceneObject> m_objects = new List<SceneObject>();

        private Camera m_camera = new Camera();

        public Scene(IAPILayer apiLayer)
        {
            m_apiLayer = apiLayer;

            m_apiLayer.OnResize += Resize;

            m_pipeline = m_apiLayer.CreatePipeline(cfg =>
            {
                cfg.UseInputFormat<VectorFormatPCT>();

                cfg.UseCulling(CullMode.None);
                //cfg.EnableDepthTest(false);
                cfg.UsePrimitive(PrimitiveType.TriangleStrip);

                cfg.AddShaderCode(ShaderType.Vertex, VERTEX);
                cfg.AddShaderCode(ShaderType.Fragment, FRAGMENT);
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

        public Camera Camera
        {
            get => m_camera;
            set => m_camera = value ?? new Camera();
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
            float w = size.X;
            float h = size.Y;

            Projection = Matrix4x4.CreatePerspectiveFieldOfView((float)MathX.DegToRad(45), w / h, 0.1f, 100f);
        }

        public void Render()
        {
            m_pipeline.Activate(m_commandList);

            using var cmdScope = m_commandList.BeginScope();
            m_commandList.ClearBuffers(GlobalBuffer.ColorBuffer | GlobalBuffer.DepthBuffer | GlobalBuffer.StencilBuffer);

            m_pipeline.Uniforms.projection = Projection;
            m_pipeline.Uniforms.view = m_camera.View;

            foreach (var obj in m_objects)
            {
                m_pipeline.Uniforms.model = obj.Model;
                obj.Render(m_commandList);
            }
        }
    }
}