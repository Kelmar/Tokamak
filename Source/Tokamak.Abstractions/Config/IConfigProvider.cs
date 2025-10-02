using System.Collections.Generic;

namespace Tokamak.Config.Abstractions
{
    public interface IConfigProvider
    {
        public IEnumerable<KeyValuePair<string, string>> GetValues();
    }
}
