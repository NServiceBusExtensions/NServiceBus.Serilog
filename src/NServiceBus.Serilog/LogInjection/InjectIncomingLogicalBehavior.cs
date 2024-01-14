class InjectIncomingLogicalBehavior(LogBuilder builder) :
    Behavior<IIncomingLogicalMessageContext>
{
    public class Registration(LogBuilder logBuilder) :
        RegisterStep(
            stepId: Name,
            behavior: typeof(InjectIncomingLogicalBehavior),
            description: "Injects a logger into the incoming logical context",
            factoryMethod: _ => new InjectIncomingLogicalBehavior(logBuilder));

    public static string Name = $"Serilog{nameof(InjectIncomingLogicalBehavior)}";

    public override async Task Invoke(IIncomingLogicalMessageContext context, Func<Task> next)
    {
        var previous = context.Extensions.Get<ILogger>();
        try
        {
            await Inner(context, next);
        }
        finally
        {
            context.Extensions.Set(previous);
        }
    }

    public Task Inner(IIncomingLogicalMessageContext context, Func<Task> next)
    {
        var type = context.Message.MessageType;
        var typeName = TypeNameConverter.GetName(type);
        var properties = new List<PropertyEnricher>
        {
            new("IncomingMessageId", context.MessageId),
            typeName.IncomingMessageType,
            typeName.IncomingMessageTypeLong
        };
        var logger = builder.GetLogger(typeName.MessageTypeName);

        var headers = context.MessageHeaders;
        if (headers.TryGetValue(Headers.CorrelationId, out var correlationId))
        {
            properties.Add(new("CorrelationId", correlationId));
        }

        if (headers.TryGetValue(Headers.ConversationId, out var conversationId))
        {
            properties.Add(new("ConversationId", conversationId));
        }

        var loggerForContext = logger.ForContext(properties);
        context.Extensions.Set(loggerForContext);

        return next();
    }
}