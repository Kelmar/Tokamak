using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Collections.ObjectModel;
using System.Runtime.CompilerServices;
using System.Drawing;

namespace Tokamak.Formats
{
    /// <summary>
    /// Class for getting information on a vetex buffer layout.
    /// </summary>
    /// <remarks>
    /// This class takes in a structure and builds up information about how it is laid out in memory so that we 
    /// can send that over to the undelying video system.
    /// 
    /// What this effectively does is gives us a way to define an arbitrary structure and map it to our shader parameters.
    /// </remarks>
    public static class VectorFormat
    {
        /// <summary>
        /// Detailed info about a given structure.
        /// </summary>
        public class Info
        {
            public Info(Type type)
            {
                Type = type;

                Size = Marshal.SizeOf(Type);

                Items = ParseItems().AsReadOnly();
            }

            private List<ItemInfo> ParseItems()
            {
                var fields = Type.GetFields(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);

                /*
                 * Note that if an item doesn't have a FormatDesciptorAttribute, we'll throw a NullReferenceException.
                 * That is intentional, we want this to be a required attribute when working with vertex buffers.
                 * 
                 * TODO: Look into a nicer way to verify this and provide a clearer error.
                 */

                var firstPass =
                (
                    from field in fields
                    let offset = Marshal.OffsetOf(Type, field.Name)
                    let attr = Type.GetCustomAttribute<FormatDescriptorAttribute>()
                    orderby offset
                    select new ItemInfo
                    {
                        Index = -1, // We fill this in later.
                        Offset = offset,
                        BaseType = attr.BaseType,
                        DeclaringType = field.DeclaringType,
                        Count = attr.Count
                    }
                ).ToList();

                return firstPass
                    .Select((item, index) => new ItemInfo
                    {
                        Index = index,
                        Offset = item.Offset,
                        Stride = ComputeStride(item, firstPass),
                        BaseType = item.BaseType,
                        Count = item.Count
                    })
                    .ToList();
            }

            private int ComputeStride(ItemInfo item, List<ItemInfo> allItems)
            {
                return allItems.Where(i => i != item).Sum(i => Marshal.SizeOf(i.DeclaringType));
            }

            /// <summary>
            /// The structure's Type object, for reference.
            /// </summary>
            public Type Type { get; }

            /// <summary>
            /// The size of this structure in bytes.
            /// </summary>
            public int Size { get; }

            /// <summary>
            /// The structure's name, for reference.
            /// </summary>
            public string Name => Type.Name;

            /// <summary>
            /// Details on the layout of each item in the structure.
            /// </summary>
            public ReadOnlyCollection<ItemInfo> Items { get; }
        }

        /// <summary>
        /// Information about a single field in a layout structure.
        /// </summary>
        public class ItemInfo
        {
            /// <summary>
            /// The ordering of this item.
            /// </summary>
            public int Index { get; set; }

            /// <summary>
            /// Offset from the begining of the structure the item occurs at.
            /// </summary>
            public nint Offset { get; set; }

            /// <summary>
            /// The amount to adjust the offset by each time accessing this element.
            /// </summary>
            public int Stride { get; set; }

            /// <summary>
            /// The basic graphics type this item holds.
            /// </summary>
            /// <remarks>
            /// For most Vectors for example this is a float, but could be an int, a byte, or whatever.
            /// </remarks>
            public FormatBaseType BaseType { get; set; }

            /// <summary>
            /// The type that was actually declared in the structure.
            /// </summary>
            public Type DeclaringType { get; set; }

            /// <summary>
            /// The number of BaseType items this field holds.
            /// </summary>
            /// <remarks>
            /// Usually from 1 to 4
            /// </remarks>
            public int Count { get; set; }
        }

        // TODO: Make concurrent?
        private readonly static IDictionary<Type, Info> s_layouts = new Dictionary<Type, Info>();

        static VectorFormat()
        {
            // Prebuild for some common base structures.
            var items = new List<Type>
            {
                typeof(VectorFormatPC),
                typeof(VectorFormatPCT)
            };

            foreach (Type type in items)
                s_layouts[type] = new Info(type);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Info GetLayoutOf<T>()
            where T : struct
        {
            Type t = typeof(T);

            if (s_layouts.TryGetValue(t, out Info info))
                return info;

            info = new Info(t);
            s_layouts[t] = info;
            return info;
        }
    }
}
