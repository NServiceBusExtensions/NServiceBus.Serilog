using System.Threading.Tasks;
using NServiceBus.Testing;
using Serilog;
using Xunit;

public class HandlerUsingContextLoggerTests
{
    [Fact]
    public Task Simple()
    {
        var handler = new HandlerUsingContextLogger();
        var context = new TestableMessageHandlerContext();
        context.Extensions.Set(Log.Logger);
        return handler.Handle(new StartHandlerUsingContextLogger(), context);
    }
}