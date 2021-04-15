using System;
using System.Collections.Concurrent;
using System.Linq;
using System.Text;

namespace NServiceBus.Serilog
{
    /// <summary>
    /// Converts a <see cref="Type"/> or a long type name to a short type name.
    /// </summary>
    public static class TypeNameConverter
    {
        static ConcurrentDictionary<string, string> longNameToNameCache = new();

        /// <summary>
        /// Get a short type name from a long type name.
        /// </summary>
        public static string GetName(string longName)
        {
            return longNameToNameCache.GetOrAdd(longName, FormatForDisplay);
        }

        static string FormatForDisplay(string longName)
        {
            var parsedName = TypeNameParser.ParseName(longName,0,out _);
            StringBuilder builder = new();
            FormatForDisplay(parsedName!,builder);
            builder.Remove(0, 1);
            builder.Remove(builder.Length-1, 1);
            var formatForDisplay = builder.ToString();
            return formatForDisplay;
        }

        static void FormatForDisplay(ParsedName name, StringBuilder builder)
        {
            builder.Append($"<{name.Names.First().Name}");
            foreach (var typeName in name.Names.Skip(1))
            {
                builder.Append($"+{typeName.Name}");
            }

            foreach (var typeArgument in name.TypeArguments)
            {
                FormatForDisplay(typeArgument, builder);
            }

            builder.Append(">");
        }
    }
}