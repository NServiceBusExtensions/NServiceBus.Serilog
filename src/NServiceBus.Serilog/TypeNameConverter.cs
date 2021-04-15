using System;
using System.Collections.Concurrent;
using System.Text;

namespace NServiceBus.Serilog
{
    /// <summary>
    /// Converts a <see cref="Type"/> or a long type name to a short type name.
    /// </summary>
    public static class TypeNameConverter
    {
        static ConcurrentDictionary<string, string> longNameToNameCache = new();
        static ConcurrentDictionary<Type, string> typeToNameCache = new();

        /// <summary>
        /// Get a short type name from a long type name.
        /// </summary>
        public static string GetName(string longName)
        {
            return longNameToNameCache.GetOrAdd(longName, FormatForDisplay);
        }

        /// <summary>
        /// Get a short type name from a long type name.
        /// </summary>
        public static string GetName(Type type)
        {
            return typeToNameCache.GetOrAdd(type, FormatForDisplay);
        }

        static string FormatForDisplay(string typeName)
        {
            var type = Type.GetType(typeName);
            if (type == null)
            {
                return typeName;
            }

            return FormatForDisplay(type);
        }

        static string FormatForDisplay(Type type)
        {
            StringBuilder builder = new();
            FormatForDisplay(type, builder);
            return builder.ToString();
        }

        static void FormatForDisplay(Type type, StringBuilder builder)
        {
            if (type.IsNested)
            {
                FormatForDisplay(type.DeclaringType!, builder);
                builder.Append('+');
            }

            var typeName = type.Name;

            var indexOfGenericDelimiter = typeName.IndexOf('`');
            if (indexOfGenericDelimiter != -1)
            {
                typeName = typeName.Substring(0, indexOfGenericDelimiter);
            }

            builder.Append(typeName);
            if (type.IsGenericType)
            {
                builder.Append('<');
                foreach (var typeArgument in type.GenericTypeArguments)
                {
                    FormatForDisplay(typeArgument, builder);
                }

                builder.Append('>');
            }
        }
    }
}