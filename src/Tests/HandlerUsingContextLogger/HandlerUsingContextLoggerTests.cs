using System.Threading.Tasks;
using NServiceBus.Testing;
using Xunit;

public class HandlerUsingContextLoggerTests
{
    [Fact]
    public Task Simple()
    {
        var handler = new HandlerUsingContextLogger();
        var context = new TestableMessageHandlerContext();
        return handler.Handle(new StartHandlerUsingContextLogger(), context);
    }
}