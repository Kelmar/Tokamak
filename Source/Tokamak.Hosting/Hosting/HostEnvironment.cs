using Tokamak.Hosting.Abstractions;

namespace Tokamak.Hosting.Implementation
{
    internal class HostEnvironment : IHostEnvironment
    {
        public required string ApplicationName { get; set; }

        public required string EnvironmentName { get; set; }
    }
}
