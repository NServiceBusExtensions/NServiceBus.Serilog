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
            try
            {
                var name = longName;
                var commaIndex = name.IndexOf(',');
                if (commaIndex > -1)
                {
                    name = name.Substring(0, commaIndex);
                }

                var dotIndex = name.IndexOf('.');
                if (dotIndex > -1)
                {
                    name = name.Substring(dotIndex+1, name.Length - dotIndex -1);
                }

                return name;
            }
            catch (Exception exception)
            {
                throw new($"Could not convert to short type name. longName: {longName}", exception);
            }
        }
    }
}