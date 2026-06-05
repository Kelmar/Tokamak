using System;
using System.Collections.Generic;
using System.Numerics;
using System.Text;

namespace Tokamak.Assets
{
    public interface IMeshBuilder
    {
        IMeshBuilder WithName(string name);

        IPolygonBuilder GetPolygonBuilder();
    }
}
