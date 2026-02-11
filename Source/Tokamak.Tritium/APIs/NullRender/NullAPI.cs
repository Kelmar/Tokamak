namespace Tokamak.Tritium.APIs.NullRender
{
    internal class NullAPI : IAPIDescriptor
    {
        public NullAPI()
        {
        }

        public string ID => "null";

        public string Name => "Null Renderer";

        public SupportLevel SupportLevel => SupportLevel - 100; // Should never automatically choose this.

        public IGraphicsLayer Build() => new NullLayer();
    }
}
