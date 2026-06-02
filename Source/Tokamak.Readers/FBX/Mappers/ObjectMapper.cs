using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Reflection;

using Tokamak.Readers.FBX.DOM;

namespace Tokamak.Readers.FBX.Mappers
{
    internal static class ObjectMapper
    {
        // TODO: Some sort of path mapping to make this cleaner? -- B.Simonds (June 2, 2026)

        public static T MapTo<T>(this IEnumerable<ObjectProperty> fbxProps)
            where T : class, new()
        {
            Type type = typeof(T);
            var typeProps = type.GetProperties().Where(p => p.CanWrite && p.CanRead);

            T result = new();

            foreach (var typeProp in typeProps)
            {
                var notMapped = typeProp.GetCustomAttribute<NotMappedAttribute>();

                if (notMapped != null)
                    continue;

                var colAttr = typeProp.GetCustomAttribute<ColumnAttribute>();

                string name = colAttr?.Name ?? typeProp.Name;

                var fbxProp = fbxProps.FirstOrDefault(p => p.Name == name);

                if (fbxProp == null)
                    continue;

                try
                {
                    object data = Convert.ChangeType(fbxProp.Data, typeProp.PropertyType);
                    typeProp.SetValue(result, data);
                }
                catch
                {
                    // TODO: Might be worthwhile logging that we can't set the value.
                    continue;
                }
            }

            return result;
        }

        public static T MapTo<T>(this FBXObject obj)
            where T : class, new()
            => obj.Properties.MapTo<T>();
    }
}
