using System.Collections.Generic;

sealed class ParsedName
{
    public List<TypeName> Names { get; } = new();
    public List<ParsedName> TypeArguments { get; } = new();
    public List<int> Modifiers { get; } = new();
    public string? AssemblyName{ get; set; }
}

class TypeName
{
    public string? Namespace { get; }
    public string Name { get; }

    public TypeName(string? @namespace, string name)
    {
        Namespace = @namespace;
        Name = name;
    }
}