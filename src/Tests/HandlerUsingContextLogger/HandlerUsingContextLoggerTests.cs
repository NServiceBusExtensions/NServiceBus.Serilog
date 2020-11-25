using System.Threading.Tasks;
using NServiceBus.Testing;
using Xunit;

public class HandlerUsingContextLoggerTests
{
    [Fact]
    public Task Simple()
    {
        HandlerUsingContextLogger handler = new();
        TestableMessageHandlerContext context = new();
        return handler.Handle(new StartHandlerUsingContextLogger(), context);
    }
}