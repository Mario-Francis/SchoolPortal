using SchoolPortal.Core.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace SchoolPortal.Core.Extensions
{
    public static class ObjectExtension
    {
        public static ObjectChangesCheckResult CheckChanges<T>(this T oldObject, T newObject, params string[] ignore) where T : class
        {
            var fromList = new List<KeyValuePair<string, string>>();
            var toList = new List<KeyValuePair<string, string>>();
            var hasChanges = false;

            Type type = typeof(T);
            List<string> ignoreList = new List<string>(ignore);
            foreach (PropertyInfo pi in type.GetProperties(BindingFlags.Public | BindingFlags.Instance))
            {
                if (!ignoreList.Contains(pi.Name) && pi.PropertyType.IsSimpleType())
                {
                    object oldValue = type.GetProperty(pi.Name).GetValue(oldObject, null);
                    object newValue = type.GetProperty(pi.Name).GetValue(newObject, null);

                    if (oldValue != newValue && (oldValue == null || !oldValue.Equals(newValue)))
                    {
                        hasChanges = true;
                        fromList.Add(new KeyValuePair<string, string>(pi.Name, oldValue == null ? "null" : oldValue.ToString()));
                        toList.Add(new KeyValuePair<string, string>(pi.Name, newValue == null ? "null" : newValue.ToString()));
                    }
                }
            }

            return new ObjectChangesCheckResult()
            {
                HasChanges = hasChanges,
                FromProperties = fromList,
                ToProperties = toList
            };
        }

        private static bool IsSimpleType(this Type type)
        {
            return
               type.IsValueType ||
               type.IsPrimitive ||
               new[]
               {
               typeof(String),
               typeof(Decimal),
               typeof(DateTime),
               typeof(DateTimeOffset),
               typeof(TimeSpan),
               typeof(Guid)
               }.Contains(type) ||
               (Convert.GetTypeCode(type) != TypeCode.Object);
        }
    }
}
