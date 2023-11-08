using System;
using System.Collections.Generic;
using System.Numerics;
using System.Runtime.Versioning;

using Tokamak.Formats;
using Tokamak.Mathematics;

namespace Tokamak.Scenes
{
    public class Scene : IDisposable, IRenderable
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
        private readonly IPipeline m_pipeline;

        private readonly List<SceneObject> m_objects = new List<SceneObject>();

        private Camera m_camera = new Camera();

        public Scene(Platform device)
        {
            m_pipeline = device.GetPipeline(cfg =>
            {
                cfg.UseInputFormat<VectorFormatPCT>();

                cfg.UseCulling(CullMode.Back);
                //UseDepthTest = false

                //cfg.UseShader(ShaderType.Vertex, VERTEX);
                //cfg.UseShader(ShaderType.Fragment, FRAGMENT);
            });

            //using var factory = m_device.GetPipelineFactory();

            //factory.AddShader(VERTEX, ShaderType.Vertex);
            //factory.AddShader(FRAGMENT, ShaderType.Fragment);

            //m_pipeline = factory.Build();
        }

        public void Dispose()
        {
            foreach (var obj in m_objects)
                obj.Dispose();

            m_pipeline.Dispose();
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

        public void Resize(in Point size)
        {
            float w = size.X;
            float h = size.Y;

            Projection = Matrix4x4.CreatePerspectiveFieldOfView((float)MathX.DegToRad(45), w / h, 0.1f, 100f);
        }

        public void Render()
        {
#if false
            m_device.SetRenderState(m_sceneState);

            m_pipeline.Activate();

            m_pipeline.Set("projection", Projection);
            m_pipeline.Set("view", m_camera.View);

            foreach (var obj in m_objects)
            {
                m_pipeline.Set("model", obj.Model);
                obj.Render();
            }
#endif
        }
    }
}
