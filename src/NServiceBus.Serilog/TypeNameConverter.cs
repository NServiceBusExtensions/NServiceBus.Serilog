using System;
using System.Collections.Concurrent;

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

            return type.Name;
        }
    }
}