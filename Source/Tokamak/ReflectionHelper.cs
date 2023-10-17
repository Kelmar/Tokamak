using System;
using System.Linq;

namespace Tokamak
{
    public static class ReflectionHelper
    {
        public static bool Implements<TInterface>(this Type type)
        {
            Type iType = typeof(TInterface);

            if (!iType.IsInterface)
                throw new Exception($"{iType.Name} is not an interface.");

            return type.GetInterfaces().Contains(iType);
        }
    }
}
