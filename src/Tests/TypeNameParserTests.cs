using System.Threading.Tasks;
using VerifyXunit;
using Xunit;

[UsesVerify]
public class TypeNameParserTests
{
    [Fact]
    public Task Simple()
    {
        var typeName = TypeNameParser.ParseName("TheName", true, 0, out _);
        return Verifier.Verify(typeName);
    }

    [Fact]
    public Task Namespace()
    {
        var typeName = TypeNameParser.ParseName("Namespace.TheName", true, 0, out _);
        return Verifier.Verify(typeName);
    }

    [Fact]
    public Task AssemblyQualified()
    {
        var typeName = TypeNameParser.ParseName("Namespace.TheName, Assembly, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089", true, 0, out _);
        return Verifier.Verify(typeName);
    }

    [Fact]
    public Task GenericAssemblyQualified()
    {
        var typeName = TypeNameParser.ParseName("Namespace.MyMessage`1[[System.Int32, System.Private.CoreLib, Version=5.0.0.0, Culture=neutral, PublicKeyToken=7cec85d7bea7798e]], Sample, Version=0.0.0.0, Culture=neutral, PublicKeyToken=ce8ec7717ba6fbb6", true, 0, out _);
        return Verifier.Verify(typeName);
    }

    [Fact]
    public Task Generic()
    {
        var typeName = TypeNameParser.ParseName("Namespace.MyMessage`1[[System.Int32]], Sample", true, 0, out _);
        return Verifier.Verify(typeName);
    }
}