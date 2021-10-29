using System;
using System.Collections.Generic;
using System.Text;

namespace SchoolPortal.Core.Extensions
{
    public static class CollectionExtension
    {
        public static void AddRange<T>(this ICollection<T> collection, IEnumerable<T> items)
        {
            foreach (var item in items)
                collection.Add(item);
        }
    }
}
