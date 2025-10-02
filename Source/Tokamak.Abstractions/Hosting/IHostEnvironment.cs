namespace Tokamak.Hosting.Abstractions
{
    public interface IHostEnvironment
    {
        string ApplicationName { get; set; }

        string EnvironmentName { get; set; }
    }
}
