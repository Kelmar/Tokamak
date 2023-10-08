using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

using OpenTK.Graphics.OpenGL4;

using SNum = System.Numerics;
using TKNum = OpenTK.Mathematics;

namespace Tokamak.OGL
{
    internal class Shader : IShader
    {
        private readonly IDictionary<string, int> m_uniforms = new Dictionary<string, int>();

        public Shader()
        {
            Handle = GL.CreateProgram();
        }

        public void Dispose()
        {
            if (Handle != 0)
                GL.DeleteProgram(Handle);
        }

        internal int Handle { get; }

        internal void Link()
        {
            GL.LinkProgram(Handle);

            GL.GetProgram(Handle, GetProgramParameterName.LinkStatus, out int status);

            if (status != (int)All.True)
            {
                string infoLog = GL.GetProgramInfoLog(Handle);
                throw new Exception($"Error linking GLSL shader: {infoLog}");
            }

            // Find all the uniform locations for this program and save them for later use.
            GL.GetProgram(Handle, GetProgramParameterName.ActiveUniforms, out int uniformCount);

            for (int i = 0; i < uniformCount; ++i)
            {
                string key = GL.GetActiveUniform(Handle, i, out _, out _);
                int loc = GL.GetUniformLocation(Handle, key);

                m_uniforms[key] = loc;
            }
        }

        public void Activate()
        {
            GL.UseProgram(Handle);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private int GetLocation(string name) => m_uniforms[name];

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Set(string name, int value)
        {
            GL.Uniform1(GetLocation(name), value);
        }

        public void Set(string name, float value)
        {
            GL.Uniform1(GetLocation(name), value);
        }

        public void Set(string name, in SNum.Vector2 vector)
        {
            GL.Uniform2(GetLocation(name), vector.X, vector.Y);
        }

        public void Set(string name, in SNum.Vector3 vector)
        {
            GL.Uniform3(GetLocation(name), vector.X, vector.Y, vector.Z);
        }

        public void Set(string name, in SNum.Vector4 vector)
        {
            GL.Uniform4(GetLocation(name), vector.X, vector.Y, vector.Z, vector.W);
        }

        public void Set(string name, in SNum.Matrix3x2 mat)
        {
            var m = mat.ToOpenTK();
            GL.UniformMatrix3x2(GetLocation(name), false, ref m);
        }

        public void Set(string name, in SNum.Matrix4x4 mat)
        {
            var m = mat.ToOpenTK();
            GL.UniformMatrix4(GetLocation(name), false, ref m);
        }
    }
}
