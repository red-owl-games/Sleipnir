using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEngine;

namespace RedOwl.Sleipnir.Engine
{
    public static class TypeExtensions
    {
        private static BindingFlags flags = BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.FlattenHierarchy;
        
        private static readonly Dictionary<(Type, Type), bool> CastableCache = new Dictionary<(Type, Type), bool>();
        
        public static IEnumerable<Type> SafeGetTypes(this Assembly self)
        {
            try
            {
                return self.GetTypes();
            }
            catch
            {
                return Type.EmptyTypes;
            }
        }
        
        public static string SafeGetName(this Type self)
        {
            string output = "";
            var type = self;
            while (true)
            {
                output = type.FullName;
                if (string.IsNullOrEmpty(output))
                {
                    output = self.GetGenericTypeDefinition().FullName;
                    if (!string.IsNullOrEmpty(output))
                    {
                        return output;
                    }
                }
                else
                {
                    return output;
                }

                type = type.BaseType;
            }
        }
        
        public static IEnumerable<Type> GetAllTypes()
        {
            foreach (var assembly in AppDomain.CurrentDomain.GetAssemblies())
            {
                if (assembly.GlobalAssemblyCache) continue;
                foreach (var type in assembly.SafeGetTypes())
                {
                    yield return type;
                }
            }
        }

        public static IEnumerable<Type> GetAllTypes(Type match)
        {
            foreach (var type in GetAllTypes())
            {
                if (match.IsAssignableFrom(type) && !type.IsAbstract && !type.IsInterface)
                    yield return type;
            }
        }
        
        public static IEnumerable<Type> GetAllTypes<T>() => GetAllTypes(typeof(T));

        public static IEnumerable<MethodInfo> GetMethods<T>(T instance)
        {
            foreach (MethodInfo info in instance.GetType().GetMethods(flags))
            {
                yield return info;
            }
        }

        public static IEnumerable<FieldInfo> GetFields<T>(T instance)
        {
            foreach (FieldInfo info in instance.GetType().GetFields(flags))
            {
                yield return info;
            }
        }

        public static bool IsCastableTo(this Type from, Type to, bool implicitly = false)
        {
            // Based on https://stackoverflow.com/a/22031364
            var key = (from, to);
            if (CastableCache.TryGetValue(key, out bool support))
            {
                return support;
            }

            support = to.IsAssignableFrom(from) || from.HasCastDefined(to, implicitly) || typeof(string).IsAssignableFrom(to);
            CastableCache.Add(key, support);
            return support;
        }

        private static bool HasCastDefined(this Type from, Type to, bool implicitly)
        {
            if ((from.IsPrimitive || from.IsEnum) && (to.IsPrimitive || to.IsEnum))
            {
                if (!implicitly)
                {
                    return from == to || (from != typeof(bool) && to != typeof(bool));
                }
                
                Type[][] typeHierarchy = {
                    new[] { typeof(byte),  typeof(sbyte), typeof(char) },
                    new[] { typeof(short), typeof(ushort) },
                    new[] { typeof(int), typeof(uint) },
                    new[] { typeof(long), typeof(ulong) },
                    new[] { typeof(float) },
                    new[] { typeof(double) }
                };

                var lowerTypes = Enumerable.Empty<Type>();
                foreach (var types in typeHierarchy)
                {
                    if (types.Any(t => t == to))
                    {
                        return lowerTypes.Any(t => t == from);
                    }
                        
                    lowerTypes = lowerTypes.Concat(types);
                }

                return false; // IntPtr, UIntPtr, Enum, Boolean
            }

            return HasCastOperator(to, m => m.GetParameters()[0].ParameterType, _ => from, implicitly, false)
                || HasCastOperator(from, _ => to, m => m.ReturnType, implicitly, true);
        }

        private static bool HasCastOperator(
            IReflect type, Func<MethodInfo, Type> baseType, Func<MethodInfo, Type> derivedType, bool implicitly, bool lookInBase)
        {
            var bindingFlags = BindingFlags.Public | BindingFlags.Static
                            | (lookInBase ? BindingFlags.FlattenHierarchy : BindingFlags.DeclaredOnly);

            return type.GetMethods(bindingFlags).Any(
                m => (m.Name == "op_Implicit" || (!implicitly && m.Name == "op_Explicit"))
                     && baseType(m).IsAssignableFrom(derivedType(m))
            );
        }
        
        public static void WithAttr<T>(this Type self, Action<T> callback, bool inherit = true) where T : Attribute
        {
            var attrs = self.GetCustomAttributes(inherit);
            foreach (var item in attrs)
            {
                if (item is T attr) callback(attr);
            }
        }
        
        public static void WithAttr<T>(this MemberInfo self, Action<T> callback, bool inherit = true) where T : Attribute
        {
            var attrs = self.GetCustomAttributes(inherit);
            foreach (var item in attrs)
            {
                if (item is T attr) callback(attr);
            }
        }
        
        public static void WithAttr<T, TAttr>(this T self, Action<TAttr> callback, bool inhert = true) where TAttr : Attribute
        {
            self.GetType().WithAttr(callback, inhert);
        }

        public static bool TryGetAttr<T>(this Type self, out T attr, bool inhert = true) where T : Attribute
        {
            var attrs = self.GetCustomAttributes(inhert);
            foreach (var item in attrs)
            {
                if (!(item is T a)) continue;
                attr = a;
                return true;
            }

            attr = null;
            return false;
        }

        /*
        public static void ForFieldWithType<T, TType>(this T self, Action<FieldInfo, TType> callback) where TType : IPort
        {
            foreach (var info in GetFields(self))
            {
                if (typeof(TType).IsAssignableFrom(info.FieldType)) callback(info, (TType) info.GetValue(self));
            }
        }
        */
        
        public static void ForFieldWithAttr<T, TAttr>(this T self, Action<TAttr, FieldInfo> callback, bool inhert = true) where TAttr : Attribute
        {
            foreach (var info in GetFields(self))
            {
                info.WithAttr<TAttr>((attr) => { callback(attr, info); }, inhert);
            }
        }

        public static void ForMethodWithAttr<T, TAttr>(this T self, Action<TAttr, MethodInfo> callback, bool inhert = true) where TAttr : Attribute
        {
            foreach (var info in GetMethods(self))
            {
                info.WithAttr<TAttr>((attr) => { callback(attr, info); }, inhert);
            }
        }
        
        public static Dictionary<string, FieldInfo> GetFieldTable(this Type self, BindingFlags bindingFlags)
        {
            var output = new Dictionary<string, FieldInfo>();
            foreach (var info in self.GetFields(bindingFlags).OrderBy(field => field.MetadataToken))
            {
                if (output.ContainsKey(info.Name)) continue;
                output.Add(info.Name, info);
            }

            return output;
        }
        
        public static Dictionary<string, PropertyInfo> GetPropertyTable(this Type self, BindingFlags bindingFlags)
        {
            var output = new Dictionary<string, PropertyInfo>();
            foreach (var info in self.GetProperties(bindingFlags).OrderBy(field => field.MetadataToken))
            {
                if (output.ContainsKey(info.Name)) continue;
                output.Add(info.Name, info);
            }

            return output;
        }
        
        public static Dictionary<string, MethodInfo> GetMethodTable(this Type self, BindingFlags bindingFlags)
        {
            var output = new Dictionary<string, MethodInfo>();
            foreach (var info in self.GetMethods(bindingFlags).OrderBy(field => field.MetadataToken))
            {
                if (output.ContainsKey(info.Name)) continue;
                output.Add(info.Name, info);
            }
            return output;
        }
    }
}