using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using VerifyXunit;
using Xunit;

[UsesVerify]
public class TypeNameParserTests
{
    [Fact]
    public void AllTypes()
    {
        void Parse(string name)
        {
            ParsedName? parsed;
            try
            {
                parsed = TypeNameParser.ParseName(name, 0, out _);
            }
            catch (Exception exception)
            {
                throw new(name, exception);
            }

            if (parsed == null)
            {
                throw new(name);
            }

            if (parsed.Names == null)
            {
                throw new(name);
            }
        }

        foreach (var assembly in AppDomain.CurrentDomain.GetAssemblies())
        {
            foreach (var type in assembly.GetTypes())
            {
                Parse(type.Name);
                Parse(type.FullName!);
                Parse(type.AssemblyQualifiedName!);
            }
        }
    }

    [Fact]
    public Task GenericArguments()
    {
        var type = typeof(IEnumerable<>)
            .GetGenericArguments()
            .First();
        return Verify(type);
    }

    [Fact]
    public Task Pointers()
    {
        var type = typeof(int*);
        return Verify(type);
    }

    [Fact]
    public Task Nullable()
    {
        return Verify(typeof(int?));
    }

    [Fact]
    public Task Array()
    {
        return Verify(typeof(int[]));
    }

    [Fact]
    public Task List()
    {
        return Verify(typeof(List<int>));
    }

    [Fact]
    public Task Dynamic()
    {
        return Verify(new {Name = "foo"}.GetType());
    }

    static Task Verify(Type type, [CallerFilePath] string sourceFile = "")
    {
        var name = TypeNameParser.ParseName(type.Name, 0, out _);

        ParsedName? fullName = null;
        if (type.FullName != null)
        {
            fullName = TypeNameParser.ParseName(type.FullName, 0, out _);
        }

        ParsedName? assemblyQualifiedName = null;
        if (type.AssemblyQualifiedName != null)
        {
            assemblyQualifiedName = TypeNameParser.ParseName(type.AssemblyQualifiedName, 0, out _);
        }

        return Verifier.Verify(new {name, fullName, assemblyQualifiedName}, sourceFile: sourceFile);
    }

    [Fact]
    public Task Nested()
    {
        var typeName = TypeNameParser.ParseName("Interop+Kernel32", 0, out _);
        return Verifier.Verify(typeName);
    }

    [Fact]
    public Task Simple()
    {
        var typeName = TypeNameParser.ParseName("TheName", 0, out _);
        return Verifier.Verify(typeName);
    }

    [Fact]
    public Task Namespace()
    {
        var typeName = TypeNameParser.ParseName("Namespace.TheName", 0, out _);
        return Verifier.Verify(typeName);
    }

    [Fact]
    public Task AssemblyQualified()
    {
        var typeName = TypeNameParser.ParseName("Namespace.TheName, Assembly, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089", 0, out _);
        return Verifier.Verify(typeName);
    }

    [Fact]
    public Task GenericAssemblyQualified()
    {
        var typeName = TypeNameParser.ParseName("Namespace.MyMessage`1[[System.Int32, System.Private.CoreLib, Version=5.0.0.0, Culture=neutral, PublicKeyToken=7cec85d7bea7798e]], Sample, Version=0.0.0.0, Culture=neutral, PublicKeyToken=ce8ec7717ba6fbb6", 0, out _);
        return Verifier.Verify(typeName);
    }

    [Fact]
    public Task Generic()
    {
        var typeName = TypeNameParser.ParseName("Namespace.MyMessage`1[[System.Int32]], Sample", 0, out _);
        return Verifier.Verify(typeName);
    }
}