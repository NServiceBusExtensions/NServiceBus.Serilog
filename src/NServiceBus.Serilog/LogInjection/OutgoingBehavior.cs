class OutgoingBehavior :
    Behavior<IOutgoingLogicalMessageContext>
{
    public override async Task Invoke(IOutgoingLogicalMessageContext context, Func<Task> next)
    {
        var headers = context.Headers;
        var type = context.Message.Instance.GetType();
        var messageTypeName = TypeNameConverter.GetName(type).MessageTypeName;

        var properties = new List<PropertyEnricher>
        {
            new("OutgoingMessageType", messageTypeName),
            new("OutgoingMessageId", context.MessageId),
            new("OutgoingMessageType", messageTypeName)
        };

        //TODO: log CorrelationId and ConversationId as only if diff from incoming and use different keys
        if (headers.TryGetValue(Headers.CorrelationId, out var correlationId))
        {
            properties.Add(new("CorrelationId", correlationId));
        }

        if (headers.TryGetValue(Headers.ConversationId, out var conversationId))
        {
            properties.Add(new("ConversationId", conversationId));
        }

        //TODO: log other headers

        using (LogContext.Push(properties))
        {
            await next();
        }
    }

    public class Registration() :
        RegisterStep(
            stepId: $"Serilog{nameof(OutgoingBehavior)}",
            behavior: typeof(OutgoingBehavior),
            description: "Injects a logger into the outgoing context",
            factoryMethod: _ => new OutgoingBehavior());
}