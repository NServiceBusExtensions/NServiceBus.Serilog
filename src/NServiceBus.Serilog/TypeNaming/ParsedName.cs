using System.Collections.Generic;

public sealed class ParsedName
{
    public List<string>? Names;
    public List<ParsedName>? TypeArguments;

    public List<int>? Modifiers
    {
        get => modifiers;
        set
        {
            modifiers = value;
        }
    }

    public string? AssemblyName;
    private List<int>? modifiers;
}