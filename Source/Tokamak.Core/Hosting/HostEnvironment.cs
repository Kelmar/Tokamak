namespace Tokamak.Core.Hosting
{
    internal class HostEnvironment : IHostEnvironment
    {
        public string ApplicationName { get; set; }

        public string EnvironmentName { get; set; }
    }
}
