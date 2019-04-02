using System.Threading.Tasks;
using NServiceBus.Testing;
using Serilog;
using Xunit;
using Xunit.Abstractions;

public class HandlerUsingContextLoggerTests :
    TestBase
{
    [Fact]
    public async Task Simple()
    {
        var handler = new HandlerUsingContextLogger();
        var context = new TestableMessageHandlerContext();
        context.Extensions.Set(Log.Logger);
        await handler.Handle(new StartHandlerUsingContextLogger(), context);
    }

    public HandlerUsingContextLoggerTests(ITestOutputHelper output) :
        base(output)
    {
    }
}