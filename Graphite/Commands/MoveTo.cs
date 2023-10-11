using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace Graphite.Commands
{
    class MoveTo : ICommand
    {
        public Vector2 Location { get; set; }
    }
}
