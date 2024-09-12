using System.Collections.Generic;

namespace Tokamak.Core.Config
{
    public interface IConfigProvider
    {
        public IEnumerable<KeyValuePair<string, string>> GetValues();
    }
}
