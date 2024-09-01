using Silk.NET.Core.Contexts;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tokamak.Core
{
    public interface IOGLContextProvider
    {
        IGLContextSource Context { get; }
    }
}
