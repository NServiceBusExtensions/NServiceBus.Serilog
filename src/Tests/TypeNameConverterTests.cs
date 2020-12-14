using NServiceBus.Serilog;
using VerifyXunit;
using Xunit;

[UsesVerify]
public class TypeNameConverterTests
{
    [Fact]
    public void NameOnly()
    {
        Assert.Equal("TheName", TypeNameConverter.GetName("TheName"));
    }

    [Fact]
    public void NameAndNamespace()
    {
        Assert.Equal("TheName", TypeNameConverter.GetName("Namespace.TheName"));
    }

    [Fact]
    public void AssemblyQualified()
    {
        Assert.Equal("TheName", TypeNameConverter.GetName("Namespace.TheName, Assembly, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089"));
    }

    [Fact]
    public void AssemblyQualifiedWithNoVersion()
    {
        Assert.Equal("TheName", TypeNameConverter.GetName("Namespace.TheName, Assembly"));
    }
}