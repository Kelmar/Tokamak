namespace Tokamak.Core.Implementation
{
    internal class HostEnvironment : IHostEnvironment
    {
        public string ApplicationName { get; set; }

        public string EnvironmentName { get; set; }
    }
}
