using System;
using System.Diagnostics;
using System.Linq;

namespace Tokamak.Utilities
{
    public static class ReflectionHelper
    {
        public static bool Implements<TInterface>(this Type type)
        {
            Type iType = typeof(TInterface);
            Debug.Assert(iType.IsInterface, $"{iType.Name} is not an interface.");
            return type.GetInterfaces().Contains(iType);
        }
    }
}
