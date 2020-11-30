using System;
using System.CodeDom;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using Microsoft.CSharp;

namespace NServiceBus.Serilog
{
    public static class TypeNameConverter
    {
        static ConcurrentDictionary<Type, string> typeToNameCache = new();
        static ConcurrentDictionary<string, string> longNameToNameCache = new();

        static CSharpCodeProvider codeDomProvider = new();

        public static string GetName(string longName)
        {
            return longNameToNameCache.GetOrAdd(longName, Inner);
        }

        static string Inner(string longName)
        {
            Type? type;
            try
            {
                type = Type.GetType(longName);
            }
            catch
            {
                return longName;
            }

            if (type == null)
            {
                return longName;
            }

            return GetName(type);
        }

        public static string GetName(Type type)
        {
            return typeToNameCache.GetOrAdd(type, Inner);
        }

        static string Inner(Type type)
        {
            if (IsAnonType(type))
            {
                return "dynamic";
            }

            if (type.Name.StartsWith("<") ||
                type.IsNested && type.DeclaringType == typeof(Enumerable))
            {
                var singleOrDefault = type.GetInterfaces()
                    .SingleOrDefault(x =>
                        x.IsGenericType &&
                        x.GetGenericTypeDefinition() == typeof(IEnumerable<>));
                if (singleOrDefault != null)
                {
                    if (singleOrDefault.GetGenericArguments().Single().IsAnonType())
                    {
                        return "IEnumerable<dynamic>";
                    }

                    return GetName(singleOrDefault);
                }
            }

            var typeName = GetTypeName(type);
            CodeTypeReference reference = new(typeName);
            var name = codeDomProvider.GetTypeOutput(reference);
            List<string> list = new();
            AllGenericArgumentNamespace(type, list);
            foreach (var ns in list.Distinct())
            {
                name = name.Replace($"<{ns}.", "<");
                name = name.Replace($", {ns}.", ", ");
            }

            return name;
        }

        static string GetTypeName(Type type)
        {
            if (type.FullName == null)
            {
                return type.Name;
            }

            return type.FullName.Replace(type.Namespace + ".", "");
        }

        static bool IsAnonType(this Type type)
        {
            return type.Name.Contains("AnonymousType");
        }

        static void AllGenericArgumentNamespace(Type type, List<string> list)
        {
            if (type.Namespace != null)
            {
                list.Add(type.Namespace);
            }

            var elementType = type.GetElementType();

            if (elementType != null)
            {
                AllGenericArgumentNamespace(elementType, list);
            }

            foreach (var generic in type.GenericTypeArguments)
            {
                AllGenericArgumentNamespace(generic, list);
            }
        }
    }
}