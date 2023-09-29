using NServiceBus.Testing;

[UsesVerify]
public class InjectIncomingLogicalBehaviorTests
{
    [Fact]
    public async Task Simple()
    {
        var logBuilder = new LogBuilder(new FakeLogger(), "endpoint");
        var behavior = new InjectIncomingLogicalBehavior(logBuilder);
        var context = BuildContext();
        await behavior.Inner(context, () => Task.CompletedTask);
        await Verify(context);
    }

    static TestableIncomingLogicalMessageContext BuildContext() =>
        new()
        {
            Message = new(new(typeof(Message1)), new Message1())
        };

    [Fact]
    public async Task WithHeaders()
    {
        var logBuilder = new LogBuilder(new FakeLogger(), "endpoint");
        var behavior = new InjectIncomingLogicalBehavior(logBuilder);
        var context = BuildContext();
        context.MessageHeaders.Add(Headers.ConversationId, Guid.NewGuid().ToString());
        context.MessageHeaders.Add(Headers.CorrelationId, Guid.NewGuid().ToString());
        await behavior.Inner(context, () => Task.CompletedTask);
        await Verify(context);
    }

    class Message1;
}