using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;

namespace Vitevic.Shared
{
    public static class ReflectionUtils
    {
        static class Nested
        {
            internal static readonly Dictionary<string, Type> TypeCache = new Dictionary<string, Type>();
            internal static readonly Dictionary<string, PropertyInfo> PropCache = new Dictionary<string, PropertyInfo>();
        }

        public static Type GetType(string assemblyName, string typeName, bool ignoreCase = false)
        {
            if( string.IsNullOrWhiteSpace(assemblyName) || string.IsNullOrWhiteSpace(typeName) )
            {
                return null;
            }

            string fullTypeName = assemblyName + ", " + typeName;
            Type type = null;
            lock( Nested.TypeCache )
            {
                if( Nested.TypeCache.TryGetValue(fullTypeName, out type) )
                {
                    return type;
                }
            }

            type = Type.GetType(fullTypeName, false, ignoreCase);
            if( type == null )
            {
                var assembly = AppDomain.CurrentDomain.GetAssemblies().FirstOrDefault(a => a.GetName().Name == assemblyName);
                if( assembly != null )
                {
                    type = assembly.GetType(typeName, false);
                }
            }

            if( type != null )
            {
                lock(Nested.TypeCache)
                {
                    Nested.TypeCache.Add(fullTypeName, type);
                }
            }

            return type;
        }

        public static bool QueryStaticPropertyValue<T>(Type type, string propertyName, out T value)
        {
            try
            {
                var prop = type.GetProperty(propertyName, BindingFlags.Static | BindingFlags.Public);
                if( prop != null )
                {
                    value = (T)prop.GetValue(null);
                    return true;
                }
            }
            catch(Exception ex)
            {
                Debug.WriteLine($"{nameof(QueryStaticPropertyValue)}('{type.FullName}', '{propertyName}') failed: {ex}");
            }

            value = default(T);
            return false;
        }

        public static bool SetStaticPropertyValue<T>(Type type, string propertyName, T value)
        {
            try
            {
                var prop = type.GetProperty(propertyName, BindingFlags.Static | BindingFlags.Public);
                if( prop != null )
                {
                    prop.SetValue(null, value);
                    return true;
                }
            }
            catch( Exception ex )
            {
                Debug.WriteLine($"{nameof(SetStaticPropertyValue)}('{type.FullName}', '{propertyName}') failed: {ex}");
            }
            return false;
        }
    }
}
