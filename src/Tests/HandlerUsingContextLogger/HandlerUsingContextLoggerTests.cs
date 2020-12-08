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
        return handler.Handle(new(), context);
    }

    [Fact]
    public Task Inherited()
    {
        HandlerUsingContextLogger handler = new();
        InheritedTestableMessageHandlerContext context = new();
        return handler.Handle(new(), context);
    }

    class InheritedTestableMessageHandlerContext :
        TestableMessageHandlerContext
    {

    }
}