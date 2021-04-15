using System.Text;

//https://github.com/dotnet/runtime/blob/main/src/mono/System.Private.CoreLib/src/System/TypeNameParser.cs
static class TypeNameParser
{
    // Ported from the C version in mono_reflection_parse_type_checked ()
    // Entries to the Names list are unescaped to internal form while AssemblyName is not, in an effort to maintain
    // consistency with our native parser. Since this function is just called recursively, that should also be true
    // for ParsedNames in TypeArguments.
    public static ParsedName? ParseName(string name, int pos, out int endPosition)
    {
        endPosition = 0;

        while (pos < name.Length && name[pos] == ' ')
        {
            pos++;
        }

        ParsedName parsedName = new();

        var name_start = pos;
        var inModifiers = false;
        while (pos < name.Length)
        {
            switch (name[pos])
            {
                case '+':
                    parsedName.Names.Add(UnescapeTypeName(name.Substring(name_start, pos - name_start)));
                    name_start = pos + 1;
                    break;
                case '\\':
                    pos++;
                    break;
                case '&':
                case '*':
                case '[':
                case ',':
                case ']':
                    inModifiers = true;
                    break;
            }

            if (inModifiers)
                break;
            pos++;
        }

        parsedName.Names.Add(UnescapeTypeName(name.Substring(name_start, pos - name_start)));

        var isbyref = false;
        var isptr = false;
        var rank = -1;

        var end = false;
        while (pos < name.Length && !end)
        {
            switch (name[pos])
            {
                case '&':
                    if (isbyref)
                        return null;
                    pos++;
                    isbyref = true;
                    isptr = false;
                    parsedName.Modifiers.Add(0);
                    break;
                case '*':
                    if (isbyref)
                        return null;
                    pos++;
                    parsedName.Modifiers.Add(-1);
                    isptr = true;
                    break;
                case '[':
                    // An array or generic arguments
                    if (isbyref)
                        return null;
                    pos++;
                    if (pos == name.Length)
                        return null;

                    if (name[pos] == ',' || name[pos] == '*' || name[pos] == ']')
                    {
                        // Array
                        var bounded = false;
                        isptr = false;
                        rank = 1;
                        while (pos < name.Length)
                        {
                            if (name[pos] == ']')
                            {
                                break;
                            }

                            if (name[pos] == ',')
                            {
                                rank++;
                            }
                            else if (name[pos] == '*') /* '*' means unknown lower bound */
                            {
                                bounded = true;
                            }
                            else
                            {
                                return null;
                            }

                            pos++;
                        }

                        if (pos == name.Length)
                        {
                            return null;
                        }

                        if (name[pos] != ']')
                        {
                            return null;
                        }

                        pos++;
                        /* bounded only allowed when rank == 1 */
                        if (bounded && rank > 1)
                        {
                            return null;
                        }

                        /* n.b. bounded needs both modifiers: -2 == bounded, 1 == rank 1 array */
                        if (bounded)
                        {
                            parsedName.Modifiers.Add(-2);
                        }

                        parsedName.Modifiers.Add(rank);
                    }
                    else
                    {
                        // Generic args
                        if (rank > 0 || isptr)
                        {
                            return null;
                        }

                        isptr = false;
                        while (pos < name.Length)
                        {
                            while (pos < name.Length && name[pos] == ' ')
                            {
                                pos++;
                            }

                            var fqname = false;
                            if (pos < name.Length && name[pos] == '[')
                            {
                                pos++;
                                fqname = true;
                            }

                            var arg = ParseName(name, pos, out pos);
                            if (arg == null)
                            {
                                return null;
                            }

                            parsedName.TypeArguments.Add(arg);

                            /*MS is lenient on [] delimited parameters that aren't fqn - and F# uses them.*/
                            if (fqname && pos < name.Length && name[pos] != ']')
                            {
                                if (name[pos] != ',')
                                {
                                    return null;
                                }

                                pos++;
                                var aname_start = pos;
                                while (pos < name.Length && name[pos] != ']')
                                {
                                    pos++;
                                }

                                if (pos == name.Length)
                                {
                                    return null;
                                }

                                while (char.IsWhiteSpace(name[aname_start]))
                                {
                                    aname_start++;
                                }

                                if (aname_start == pos)
                                {
                                    return null;
                                }

                                arg.AssemblyName = name.Substring(aname_start, pos - aname_start);
                                pos++;
                            }
                            else if (fqname && pos < name.Length && name[pos] == ']')
                            {
                                pos++;
                            }

                            if (pos < name.Length && name[pos] == ']')
                            {
                                pos++;
                                break;
                            }

                            if (pos == name.Length)
                            {
                                return null;
                            }

                            pos++;
                        }
                    }

                    break;
                case ']':
                    end = true;
                    break;

                case ',':
                    end = true;
                    break;

                default:
                    return null;
            }

            if (end)
            {
                break;
            }
        }

        endPosition = pos;
        return parsedName;
    }

    static readonly char[] SpecialChars = {',', '[', ']', '&', '*', '+', '\\'};

    static string UnescapeTypeName(string name)
    {
        if (name.IndexOfAny(SpecialChars) < 0)
        {
            return name;
        }

        StringBuilder builder = new(name.Length - 1);
        for (var i = 0; i < name.Length; ++i)
        {
            if (name[i] == '\\' && i + 1 < name.Length)
            {
                i++;
            }

            builder.Append(name[i]);
        }

        return builder.ToString();
    }
}