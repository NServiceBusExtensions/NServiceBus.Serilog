namespace NServiceBus.Serilog;

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
    public static string GetName(string longName) =>
        longNameToNameCache.GetOrAdd(longName, FormatForDisplay);

    /// <summary>
    /// Get a short type name from a long type name.
    /// </summary>
    public static string GetName(Type type) =>
        typeToNameCache.GetOrAdd(type, FormatForDisplay);

    static string FormatForDisplay(string typeName)
    {
        var type = Type.GetType(
            typeName,
            assemblyResolver: name =>
            {
                try
                {
                    return Assembly.Load(name.Name!);
                }
                catch
                {
                    return null;
                }
            },
            typeResolver: (assembly, name, ignoreCase) =>
            {
                if (assembly is null)
                {
                    return Type.GetType(typeName, false);
                }

                return assembly.GetType(name, false, ignoreCase);
            });
        if (type is null)
        {
            var indexOfComma = typeName.IndexOf(',');
            if (indexOfComma > -1)
            {
                typeName = typeName.Substring(0, indexOfComma);
            }

            var indexOfPeriod = typeName.IndexOf('.');
            if (indexOfPeriod > -1)
            {
                typeName = typeName.Substring(indexOfPeriod + 1);
            }

            return typeName;
        }

        return FormatForDisplay(type);
    }

    static string FormatForDisplay(Type type)
    {
        var builder = new StringBuilder();
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