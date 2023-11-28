using System;
using System.Linq;

namespace Helpers.GenericIO
{
    public static class StringSanitizeHelper
    {
        public static string Encode(this string value)
        {
            if (string.IsNullOrEmpty(value))
            {
                return value;
            }
            return System.Web.HttpUtility.HtmlEncode(value);
        }
        public static string Decode(this string value)
        {
            if (string.IsNullOrEmpty(value))
            {
                return value;
            }
            return System.Web.HttpUtility.HtmlDecode(value);
        }

        public static void EncodeStringProperties(object obj)
        {
            if (obj == null)
            {
                return;
            }
            Type objType = obj.GetType();
            var objProperties = objType.GetProperties();
            foreach (var memberInfo in objProperties)
            {
                if (!memberInfo.CanWrite)
                {
                    continue;
                }
                var memName = memberInfo.Name;
                var isString = memberInfo.PropertyType.FullName.ToLower().Contains("string");
                if (isString)
                {
                    bool forEncode = false;
                    var attribs = memberInfo.GetCustomAttributes(true);
                    foreach (var attr in attribs)
                    {
                        if (attr.GetType().FullName == typeof(AutoSanitizeAttribute).FullName)
                        {
                            forEncode = true;
                        }
                    }
                    if (forEncode)
                    {
                        var stringValue = memberInfo.GetValue(obj);
                        memberInfo.SetValue(obj, stringValue?.ToString().Encode());
                    }
                }
                else
                {
                    if (memberInfo.PropertyType.FullName.StartsWith("System"))
                    {
                        continue;
                    }
                    Type propertyType = memberInfo.GetType();
                    if (!propertyType.IsSubclassOf(typeof(System.ValueType)))
                    {
                        if (memberInfo.PropertyType.IsClass && propertyType.GetProperties().Any())
                        {
                            if (!memberInfo.PropertyType.IsGenericType &&
                                !memberInfo.PropertyType.IsGenericTypeDefinition &&
                                !memberInfo.PropertyType.IsAbstract &&
                                !memberInfo.PropertyType.IsArray)
                            {
                                try
                                {
                                    EncodeStringProperties(memberInfo.GetValue(obj));
                                }
                                catch { }
                            }

                        }
                    }
                }

            }
        }
    }

    public class AutoSanitizeAttribute : Attribute
    {
        public AutoSanitizeAttribute()
        {
        }
    }
}
