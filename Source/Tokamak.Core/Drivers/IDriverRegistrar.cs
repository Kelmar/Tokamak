using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tokamak.Core.Drivers
{
    public interface IDriverRegistrar
    {
        internal IDictionary<string, DriverInfo> DriverMeta { get; }
    }
}
