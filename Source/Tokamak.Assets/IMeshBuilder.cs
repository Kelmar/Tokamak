using System;
using System.Collections.Generic;

namespace Tokamak.Assets
{
    public interface IMeshBuilder
    {
        IMeshBuilder WithName(string name);

        IMeshBuilder WithPolygons<T>(IEnumerable<T> source, Action<T, IPolygonBuilder> config);
    }
}
