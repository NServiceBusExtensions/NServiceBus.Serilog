﻿using System.Collections.Generic;
using System.Text;
//https://github.com/dotnet/runtime/blob/main/src/mono/System.Private.CoreLib/src/System/TypeNameParser.cs
static class TypeNameParser
{
    // Ported from the C version in mono_reflection_parse_type_checked ()
    // Entries to the Names list are unescaped to internal form while AssemblyName is not, in an effort to maintain
    // consistency with our native parser. Since this function is just called recursively, that should also be true
    // for ParsedNames in TypeArguments.
    public static ParsedName? ParseName(string name, bool recursed, int pos, out int end_pos)
    {
        end_pos = 0;

        while (pos < name.Length && name[pos] == ' ')
        {
            pos++;
        }

        var names = new List<string>();
        var res = new ParsedName();

        var name_start = pos;
        var in_modifiers = false;
        while (pos < name.Length)
        {
            switch (name[pos])
            {
                case '+':
                    names.Add(UnescapeTypeName(name.Substring(name_start, pos - name_start)));
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
                    in_modifiers = true;
                    break;
            }

            if (in_modifiers)
                break;
            pos++;
        }

        names.Add(UnescapeTypeName(name.Substring(name_start, pos - name_start)));

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
                    if (res.Modifiers == null)
                    {
                        res.Modifiers = new List<int>();
                    }

                    res.Modifiers.Add(0);
                    break;
                case '*':
                    if (isbyref)
                        return null;
                    pos++;
                    if (res.Modifiers == null)
                    {
                        res.Modifiers = new List<int>();
                    }

                    res.Modifiers.Add(-1);
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
                        if (res.Modifiers == null)
                        {
                            res.Modifiers = new List<int>();
                        }

                        if (bounded)
                        {
                            res.Modifiers.Add(-2);
                        }

                        res.Modifiers.Add(rank);
                    }
                    else
                    {
                        // Generic args
                        if (rank > 0 || isptr)
                        {
                            return null;
                        }

                        isptr = false;
                        res.TypeArguments = new List<ParsedName>();
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

                            var arg = ParseName(name, true, pos, out pos);
                            if (arg == null)
                            {
                                return null;
                            }

                            res.TypeArguments.Add(arg);

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
                    if (recursed)
                    {
                        end = true;
                        break;
                    }

                    return null;
                case ',':
                    if (recursed)
                    {
                        end = true;
                        break;
                    }

                    pos++;
                    while (pos < name.Length && char.IsWhiteSpace(name[pos]))
                    {
                        pos++;
                    }

                    if (pos == name.Length)
                    {
                        return null;
                    }

                    res.AssemblyName = name.Substring(pos);
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

        end_pos = pos;
        res.Names = names;
        return res;
    }

    static readonly char[] SPECIAL_CHARS = {',', '[', ']', '&', '*', '+', '\\'};

    static string UnescapeTypeName(string name)
    {
        if (name.IndexOfAny(SPECIAL_CHARS) < 0)
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