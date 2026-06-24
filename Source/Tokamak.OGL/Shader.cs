using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

using Silk.NET.OpenGL;

using Tokamak.Tritium.Pipelines.Shaders;

using SNum = System.Numerics;

namespace Tokamak.OGL
{
    internal class Shader : IUniformAccess
    {
        // TODO: Look into SPIR-V shaders, should be supported by OGL, Vulkan, and DirectX 12

        private readonly GL m_gl;


        private readonly IDictionary<string, int> m_uniforms = new Dictionary<string, int>();

        public Shader(GL gl)
        {
            m_gl = gl;
            Handle = m_gl.CreateProgram();
        }

        public void Dispose()
        {
            if (Handle != 0)
                m_gl.DeleteProgram(Handle);
        }

        internal uint Handle { get; }

        internal void Link()
        {
            m_gl.LinkProgram(Handle);

            m_gl.GetProgram(Handle, ProgramPropertyARB.LinkStatus, out int status);

            if (status == 0) //(int)All.True)
            {
                string infoLog = m_gl.GetProgramInfoLog(Handle);
                throw new Exception($"Error linking GLSL shader: {infoLog}");
            }

            // Find all the uniform locations for this program and save them for later use.
            m_gl.GetProgram(Handle, ProgramPropertyARB.ActiveUniforms, out int uniformCount);

            for (uint i = 0; i < uniformCount; ++i)
            {
                string key = m_gl.GetActiveUniform(Handle, i, out _, out _);
                int loc = m_gl.GetUniformLocation(Handle, key);

                m_uniforms[key] = loc;
            }
        }

        public void Activate()
        {
            m_gl.UseProgram(Handle);
        }

        public bool HasUniform(string name) => m_uniforms.ContainsKey(name);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private int GetLocation(string name) => m_uniforms[name];

        private int GetUniformInt(int location)
        {
            m_gl.GetUniform(Handle, location, out int i);
            return i;
        }

        private float GetUniformFloat(int location)
        {
            m_gl.GetUniform(Handle, location, out float f);
            return f;
        }

        private double GetUniformDouble(int location)
        {
            m_gl.GetUniform(Handle, location, out double d);
            return d;
        }

        private SNum.Vector2 GetUniformVector2(int location)
        {
            Span<float> s = stackalloc float[2];
            m_gl.GetUniform(Handle, location, s);
            return new SNum.Vector2(s[0], s[1]);
        }

        private SNum.Vector3 GetUniformVector3(int location)
        {
            Span<float> s = stackalloc float[3];
            m_gl.GetUniform(Handle, location, s);
            return new SNum.Vector3(s[0], s[1], s[2]);
        }

        private SNum.Vector4 GetUniformVector4(int location)
        {
            Span<float> s = stackalloc float[4];
            m_gl.GetUniform(Handle, location, s);
            return new SNum.Vector4(s[0], s[1], s[2], s[3]);
        }

        private SNum.Matrix3x2 GetUniformMatrix3x2(int location)
        {
            Span<float> s = stackalloc float[6];
            m_gl.GetUniform(Handle, location, s);

            // May need to double check this, might not be in the order we expect.
            return new SNum.Matrix3x2(
                s[0], s[2], s[4],
                s[1], s[3], s[5]
            );
        }

        private SNum.Matrix4x4 GetUniformMatrix4x4(int location)
        {
            Span<float> s = stackalloc float[16];
            m_gl.GetUniform(Handle, location, s);

            // May need to double check this, might not be in the order we expect.
            return new SNum.Matrix4x4(
                s[ 0], s[ 4], s[ 8], s[12],
                s[ 1], s[ 5], s[ 9], s[13],
                s[ 2], s[ 6], s[10], s[14],
                s[ 3], s[ 7], s[11], s[15]
            );
        }

        public object GetUniform(string name, Type t)
        {
            int location = GetLocation(name);

            return t switch
            {
                Type when t == typeof(int) => GetUniformInt(location),
                Type when t == typeof(float) => GetUniformFloat(location),
                Type when t == typeof(double) => GetUniformDouble(location),
                Type when t == typeof(SNum.Vector2) => GetUniformVector2(location),
                Type when t == typeof(SNum.Vector3) => GetUniformVector3(location),
                Type when t == typeof(SNum.Vector4) => GetUniformVector4(location),
                Type when t == typeof(SNum.Matrix3x2) => GetUniformMatrix3x2(location),
                Type when t == typeof(SNum.Matrix4x4) => GetUniformMatrix4x4(location),
                _ => throw new Exception($"Cannot get uniform for type: {t}")
            };
        }

        public unsafe void SetUniform(string name, object value)
        {
            int location = GetLocation(name);

            switch (value)
            {
            case int i   : m_gl.Uniform1(location, i); break;
            case float f : m_gl.Uniform1(location, f); break;
            case double d: m_gl.Uniform1(location, d); break;

            case SNum.Vector2 v2: m_gl.Uniform2(location, v2); break;
            case SNum.Vector3 v3: m_gl.Uniform3(location, v3); break;
            case SNum.Vector4 v4: m_gl.Uniform4(location, v4); break;

            case SNum.Matrix3x2 m32: m_gl.UniformMatrix2x3(location, 1, true, (float*)&m32); break;
            case SNum.Matrix4x4 m44: m_gl.UniformMatrix4(location, 1, true, (float*)&m44); break;

            default:
                throw new Exception($"Cannot set uniform for type: {value.GetType()}");
            }
        }
    }
}
