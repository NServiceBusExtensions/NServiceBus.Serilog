using System.Collections.Generic;

public sealed class ParsedName
{
    public List<string> Names { get; } = new();
    public List<ParsedName> TypeArguments { get; } = new();
    public List<int> Modifiers { get; } = new();
    public string? AssemblyName{ get; set; }
}