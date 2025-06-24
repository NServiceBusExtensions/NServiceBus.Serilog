[TestFixture]
public class IncomingLogicalBehaviorTests
{
    [Test]
    public async Task Simple()
    {
        var behavior = new IncomingLogicalBehavior();
        var context = BuildContext();
        Recording.Start();
        await behavior.Invoke(context, TestExtensions.WriteLog);
        await Verify(context);
    }

    static TestableIncomingLogicalMessageContext BuildContext() =>
        new()
        {
            Message = new(new(typeof(Message1)), new Message1())
        };

    [Test]
    public async Task WithHeaders()
    {
        var behavior = new IncomingLogicalBehavior();
        var context = BuildContext();
        context.MessageHeaders.Add(Headers.ConversationId, Guid.NewGuid().ToString());
        context.MessageHeaders.Add(Headers.CorrelationId, Guid.NewGuid().ToString());
        Recording.Start();
        await behavior.Invoke(context, TestExtensions.WriteLog);
        await Verify(context);
    }

    class Message1;
}