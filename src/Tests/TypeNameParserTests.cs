using System.Threading.Tasks;
using VerifyTests;
using VerifyXunit;
using Xunit;

[UsesVerify]
public class TypeNameParserTests
{
    [Theory]
    [InlineData("TheName")]
    [InlineData("Namespace.TheName")]
    [InlineData("Namespace.TheName, Assembly, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089")]
    [InlineData("MyMessage`1[[System.Int32, System.Private.CoreLib, Version=5.0.0.0, Culture=neutral, PublicKeyToken=7cec85d7bea7798e]], Sample, Version=0.0.0.0, Culture=neutral, PublicKeyToken=ce8ec7717ba6fbb6")]
    public Task InlineDataUsage(string name)
    {
        VerifySettings settings = new();
        settings.UseParameters(name);
        var typeName = TypeNameParser.ParseName(name,true,0,out _);
        return Verifier.Verify(typeName, settings);
    }
}