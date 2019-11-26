using System.Threading.Tasks;
using NServiceBus.Testing;
using Serilog;
using VerifyXunit;
using Xunit;
using Xunit.Abstractions;

public class HandlerUsingContextLoggerTests :
    VerifyBase
{
    [Fact]
    public Task Simple()
    {
        var handler = new HandlerUsingContextLogger();
        var context = new TestableMessageHandlerContext();
        context.Extensions.Set(Log.Logger);
        return handler.Handle(new StartHandlerUsingContextLogger(), context);
    }

    public HandlerUsingContextLoggerTests(ITestOutputHelper output) :
        base(output)
    {
    }
}