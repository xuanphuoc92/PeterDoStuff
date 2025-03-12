using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Text;
using Dapper;

namespace PeterDoStuff.Extensions
{
    /// <summary>
    /// Extensions for functions with Type and Reflection
    /// </summary>
    public static class TypeExtensions
    {
        public static IEnumerable<string> GetPropertyNames(this object @this)
        {
            // Dynamic object (IDictionary<string,object>) is a special case for different treatment:
            if (@this is IDictionary<string, object> dynamicObj)
            {
                return dynamicObj
                    .Select(kv => kv.Key);
            }

            return @this
                .GetType()
                .GetProperties()
                .Select(pi => pi.Name);
        }

        public static object GetPropertyValue(this object @this, string propertyName)
        {
            // Dynamic object (IDictionary<string,object>) is a special case for different treatment:
            if (@this is IDictionary<string, object> dynamicObj)
            {
                return dynamicObj
                    .First(kv => kv.Key == propertyName)
                    .Value;
            }

            return @this
                .GetType()
                .GetProperty(propertyName)
                .GetValue(@this, null);
        }
    }
}
