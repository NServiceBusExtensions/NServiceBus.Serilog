using TypeNameConverter = NServiceBus.Serilog.TypeNameConverter;

[UsesVerify]
public class TypeNameConverterTests
{
    [Fact]
    public void NameOnly() =>
        Assert.Equal("TheClass", TypeNameConverter.GetName("TheClass"));

    [Fact]
    public void NameAndNamespace() =>
        Assert.Equal("TheClass", TypeNameConverter.GetName("Namespace.TheClass"));

    [Fact]
    public void NameAndNamespaceAndAssembly() =>
        Assert.Equal("TheClass", TypeNameConverter.GetName("Namespace.TheClass, Tests"));

    [Fact]
    public void AssemblyQualified()
    {
        Assert.Equal("TheClass", TypeNameConverter.GetName("Namespace.TheClass, Tests, Version=0.0.0.0, Culture=neutral, PublicKeyToken=ce8ec7717ba6fbb6"));
        Assert.Equal("TheClass", TypeNameConverter.GetName("Namespace.TheClass, Tests, Version=1.0.0.0, Culture=neutral, PublicKeyToken=ce8ec7717ba6fbb6"));
        Assert.Equal("TheClass", TypeNameConverter.GetName("Namespace.TheClass, Foo, Version=1.0.0.0, Culture=neutral, PublicKeyToken=ce8ec7717ba6fbb6"));
    }

    [Fact]
    public void AssemblyQualifiedWithNoVersion() =>
        Assert.Equal("TheClass", TypeNameConverter.GetName("Namespace.TheClass, Tests"));
}

namespace Namespace
{
    class TheClass{}
}