using System.Threading.Tasks;
using NServiceBus.Testing;
using Xunit;

public class HandlerUsingContextLoggerTests
{
    [Fact]
    public Task Simple()
    {
        var handler = new HandlerUsingContextLogger();
        TestableMessageHandlerContext context = new InheritedTestableMessageHandlerContext();
        return handler.Handle(new(), context);
    }

    [Fact]
    public Task Inherited()
    {
        var handler = new HandlerUsingContextLogger();
        var context = new InheritedTestableMessageHandlerContext();
        return handler.Handle(new(), context);
    }

    class InheritedTestableMessageHandlerContext :
        TestableMessageHandlerContext
    {

    }
}