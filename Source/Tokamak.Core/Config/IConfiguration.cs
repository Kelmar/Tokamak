using Tokamak.Core.Utilities;

namespace Tokamak.Core.Config
{
    public interface IConfiguration : IConfigSection
    {
        /// <summary>
        /// Notifier to watch for configuration changes.
        /// </summary>
        INotifier<ConfigNotice> OnChanged { get; }
    }

    /// <summary>
    /// Structure of information that is sent when a configuration option has been changed.
    /// </summary>
    public record class ConfigNotice
    {
        /// <summary>
        /// The key that was changed
        /// </summary>
        public required string Key { get; init; }

        /// <summary>
        /// The new value
        /// </summary>
        public required string Value { get; init; }
    }
}
