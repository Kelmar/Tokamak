using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tokamak.Core.Config
{
    public interface IConfigOptions<in T>
        where T : class
    {
        string Section { get; set; }
    }
}
