using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tokamak.Core
{
    public interface IGameEnvironment
    {
        public string GameName { get; set; }

        public string EnvironmentName { get; set; }
    }
}
