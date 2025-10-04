using System;

namespace Tokamak.Tritium.Pipelines.Shaders
{
    public interface IUniformAccess
    {
        /// <summary>
        /// Returns if a shader has the supplied uniform name.
        /// </summary>
        /// <param name="name">The name to check for</param>
        /// <returns>True if the uniform exists, false if not.</returns>
        bool HasUniform(string name);

        object GetUniform(string name, Type type);

        void SetUniform(string name, object value);
    }
}
