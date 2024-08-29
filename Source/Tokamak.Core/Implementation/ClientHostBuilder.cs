namespace Tokamak.Core.Implementation
{
    internal class ClientHostBuilder : GameHostBuilder
    {
        protected override IGameHost CreateHost() => new ClientGameHost(this);
    }
}
