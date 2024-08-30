using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tokamak.Core
{
    public interface IHostEnvironment
    {
        string ApplicationName { get; set; }

        string EnvironmentName { get; set; }
    }
}
