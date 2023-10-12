using Graphite;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

using Tokamak;
using Tokamak.Mathematics;

namespace TestBed
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
        private readonly Device m_device;
        private readonly IShader m_shader;

        private readonly RenderState m_sceneState;

        private readonly TestObject m_test;

        public Scene(Device device)
        {
            m_device = device;

            using var factory = m_device.GetShaderFactory();

            factory.AddShaderSource(ShaderType.Vertex, VERTEX);
            factory.AddShaderSource(ShaderType.Fragment, FRAGMENT);

            m_shader = factory.Build();

            m_sceneState = new RenderState
            {
                CullFaces = false,
                UseDepthTest = false
            };

            m_test = new TestObject(m_device);
        }

        public void Dispose()
        {
            m_test.Dispose();
            m_shader.Dispose();
        }

        public Matrix4x4 Projection { get; private set; }

        public Matrix4x4 View { get; private set; }

        public Matrix4x4 Model { get; private set; }

        public void Resize(in Point size)
        {
            float w = size.X;
            float h = size.Y;

            Projection = Matrix4x4.CreatePerspectiveFieldOfView((float)MathX.DegToRad(45), w / h, 0.1f, 100f);

            var camPos = new Vector3(0, 0, 30);
            var camTarget = Vector3.Zero;
            var camDir = Vector3.Normalize(camPos - camTarget);
            var camRight = Vector3.Normalize(Vector3.Cross(Vector3.UnitY, camDir));
            var camUp = Vector3.Cross(camDir, camRight);

            View = Matrix4x4.CreateLookAt(camPos, camTarget, camUp);

            Model = Matrix4x4.Identity;
        }

        public void Render()
        {
            m_device.SetRenderState(m_sceneState);

            m_shader.Activate();

            m_shader.Set("model", Model);
            m_shader.Set("view", View);
            m_shader.Set("projection", Projection);

            m_test.Render();
        }
    }
}
