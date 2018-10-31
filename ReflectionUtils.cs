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

        public static void CallMethod(object instance, string methodName)
        {
            try
            {
                var method = GetMethodInfo(instance, methodName);
                if (method != null)
                {
                    method.Invoke(instance, null);
                }
            }
            catch (Exception exception)
            {
                // TODO: Log exception;
            }
        }

        public static TResult CallMethod<TResult>(object instance, string methodName)
        {
            TResult result = default(TResult);
            try
            {
                var method = GetMethodInfo(instance, methodName);
                if (method != null)
                {
                    result = (TResult)method.Invoke(instance, null);
                }
            }
            catch (Exception exception)
            {
                // TODO: Log exception;
            }

            return result;
        }

        public static bool HasProperty(object instance, string propertyName)
        {
            return GetPropertyInfo(instance, propertyName) != null;
        }

        public static object GetPropertyValue(object instance, string propertyName)
        {
            object result = null;
            try
            {
                var property = GetPropertyInfo(instance, propertyName);
                if (property != null)
                {
                    return property.GetValue(instance);
                }
            }
            catch (Exception exception)
            {
                // TODO: Log exception;
            }

            return result;
        }
        public static TResult GetPropertyValue<TResult>(object instance, string propertyName)
        {
            return (TResult)GetPropertyValue(instance, propertyName);
        }

        public static MethodInfo GetMethodInfo(object instance, string methodName)
        {
            var type = instance.GetType();
            // try public first
            var method = type.GetMethod(methodName);
            if (method == null)
            {
                method = type.GetMethod(methodName, BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.FlattenHierarchy);
            }

            return method;
        }

        public static PropertyInfo GetPropertyInfo(object instance, string propertyName)
        {
            Type type = instance.GetType();
            PropertyInfo property = type.GetProperty(propertyName);
            if (property == null)
            {
                property = type.GetProperty(propertyName, BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.FlattenHierarchy);
            }
            if( property == null )
            {
                var allProperties = type.GetInterfaces().Select(i => i.GetProperty(propertyName));
                property = allProperties.FirstOrDefault();
            }
            return property;
        }
    }
}
