using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tokamak.Core.Logging
{
    public interface ILogFactory
    {
        ILogger GetLogger(string name);

        ILogger<T> GetLogger<T>();
    }
}
