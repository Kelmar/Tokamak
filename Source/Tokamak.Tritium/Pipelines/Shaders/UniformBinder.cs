using System;
using System.Dynamic;

namespace Tokamak.Tritium.Pipelines.Shaders
{
    /// <summary>
    /// Provides dynamic binding access to pipeline uniforms.
    /// </summary>
    public class UniformBinder : DynamicObject
    {
        private readonly IUniformAccess m_owner;

        public UniformBinder(IUniformAccess owner)
        {
            m_owner = owner;
        }

        public override bool TryGetMember(GetMemberBinder binder, out object? result)
        {
            if (!m_owner.HasUniform(binder.Name))
            {
                result = null;
                return false;
            }

            Type t = binder.ReturnType;
            bool isBool = t == typeof(bool);

            if (isBool)
                t = typeof(int); // Fetch as integer

            result = m_owner.GetUniform(binder.Name, t);

            if (isBool)
                result = !result.Equals(0); // Convert integer back to bool

            return true;
        }

        public override bool TrySetMember(SetMemberBinder binder, object? value)
        {
            if (value == null)
                return false;

            if (!m_owner.HasUniform(binder.Name))
                return false;

            if (value is bool b)
                value = b ? 1 : 0; // Convert boolean to number that GPU can handle.

            m_owner.SetUniform(binder.Name, value);
            return true;
        }
    }
}
