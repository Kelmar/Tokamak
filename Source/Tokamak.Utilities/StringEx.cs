using System;
using System.Collections.Generic;
using System.Text;

namespace Tokamak.Utilities
{
    public static class StringEx
    {
        extension (string s)
        {
            public string Truncate(int max)
            {
                return (s.Length > max) ?
                    s.Substring(0, max) :
                    s;
            }
        }
    }
}
