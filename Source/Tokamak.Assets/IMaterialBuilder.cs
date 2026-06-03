using System;
using System.Collections.Generic;
using System.Text;

namespace Tokamak.Assets
{
    public interface IMaterialBuilder
    {
        IMaterialBuilder WithName(string name);
    }
}
