namespace Tokamak.Core.Config
{
    /// <summary>
    /// This is a really simple config system for now.
    /// </summary>
    public interface IConfigReader
    {
        public T Get<T>(string name, T defValue = default);
    }
}
