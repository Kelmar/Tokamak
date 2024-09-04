using System;

using Tokamak.Core.Utilities;

namespace Tokamak.Tritium.APIs.NullRender
{
    internal class NullAPI : IAPIDescriptor
    {
        public string ID => "null";

        public string Name => "Null Renderer";

        public SupportLevel SupportLevel => SupportLevel - 100; // Should never automatically choose this.

        public IDisposable Create()
        {
            return Indisposable.Instance;
        }
    }
}
