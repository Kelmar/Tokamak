using System;
using System.Collections.Generic;
using System.Text;

namespace Tokamak.Assets
{
    public interface IModelBuilder
    {
        public IModelBuilder WithName(string name);
    }
}
