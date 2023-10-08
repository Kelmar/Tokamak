using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tokamak
{
    public interface IShaderFactory : IDisposable
    {
        void AddShaderSource(ShaderType type, string source);

        void AddShaderSource(ShaderType type, Stream source)
        {
            using var sr = new StreamReader(source);
            AddShaderSource(type, sr.ReadToEnd());
        }

        IShader Build();
    }
}
