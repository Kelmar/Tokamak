using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace Tokamak
{
    public interface IShader
    {
        public void SetVector4(string name, ref Vector4 vector);
    }
}
