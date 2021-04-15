using System.Collections.Generic;

public sealed class ParsedName
{
    public List<string>? Names;
    public List<ParsedName>? TypeArguments;
    public List<int>? Modifiers;
    public string? AssemblyName;
}