using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

using Silk.NET.OpenGL;

using SNum = System.Numerics;

namespace Tokamak.OGL
{
    internal class Shader
    {
        private readonly IDictionary<string, int> m_uniforms = new Dictionary<string, int>();

        private readonly GLPlatform m_parent;

        public Shader(GLPlatform device)
        {
            m_parent = device;
            Handle = m_parent.GL.CreateProgram();
        }

        public void Dispose()
        {
            if (Handle != 0)
                m_parent.GL.DeleteProgram(Handle);
        }

        internal uint Handle { get; }

        internal void Link()
        {
            m_parent.GL.LinkProgram(Handle);

            m_parent.GL.GetProgram(Handle, ProgramPropertyARB.LinkStatus, out int status);

            if (status == 0) //(int)All.True)
            {
                string infoLog = m_parent.GL.GetProgramInfoLog(Handle);
                throw new Exception($"Error linking GLSL shader: {infoLog}");
            }

            // Find all the uniform locations for this program and save them for later use.
            m_parent.GL.GetProgram(Handle, ProgramPropertyARB.ActiveUniforms, out int uniformCount);

            for (uint i = 0; i < uniformCount; ++i)
            {
                string key = m_parent.GL.GetActiveUniform(Handle, i, out _, out _);
                int loc = m_parent.GL.GetUniformLocation(Handle, key);

                m_uniforms[key] = loc;
            }
        }

        public void Activate()
        {
            m_parent.GL.UseProgram(Handle);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private int GetLocation(string name) => m_uniforms[name];

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Set(string name, int value)
        {
            m_parent.GL.Uniform1(GetLocation(name), value);
        }

        public void Set(string name, float value)
        {
            m_parent.GL.Uniform1(GetLocation(name), value);
        }

        public void Set(string name, in SNum.Vector2 vector)
        {
            m_parent.GL.Uniform2(GetLocation(name), vector);
        }

        public void Set(string name, in SNum.Vector3 vector)
        {
            m_parent.GL.Uniform3(GetLocation(name), vector);
        }

        public void Set(string name, in SNum.Vector4 vector)
        {
            m_parent.GL.Uniform4(GetLocation(name), vector);
        }

        public unsafe void Set(string name, SNum.Matrix3x2 mat)
        {
            //var m = mat.ToOpenTK();
            m_parent.GL.UniformMatrix3x2(GetLocation(name), 1, true, (float*)&mat);
        }

        public unsafe void Set(string name, SNum.Matrix4x4 mat)
        {
            //var m = mat.ToOpenTK();
            m_parent.GL.UniformMatrix4(GetLocation(name), 1, true, (float*)&mat);
        }
    }
}
