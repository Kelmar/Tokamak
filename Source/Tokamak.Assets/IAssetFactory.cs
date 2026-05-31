using System;

namespace Tokamak.Assets
{
    public interface IAssetFactory
    {
        Type ForType { get; }

        Asset Build(string path);
    }
}
