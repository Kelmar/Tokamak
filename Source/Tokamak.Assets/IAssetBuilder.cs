using System;
using System.Collections.Generic;
using System.Text;

namespace Tokamak.Assets
{
    public interface IAssetBuilder
    {
        void AddModel(Action<IModelBuilder> configure);

    }
}
