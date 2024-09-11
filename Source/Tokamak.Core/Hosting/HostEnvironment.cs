namespace Tokamak.Core.Hosting
{
    internal class HostEnvironment : IHostEnvironment
    {
        public required string ApplicationName { get; set; }

        public required string EnvironmentName { get; set; }
    }
}
