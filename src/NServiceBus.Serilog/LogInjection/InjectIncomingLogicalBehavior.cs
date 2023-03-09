class InjectIncomingLogicalBehavior :
    Behavior<IIncomingLogicalMessageContext>
{
    LogBuilder logBuilder;

    public InjectIncomingLogicalBehavior(LogBuilder logBuilder) =>
        this.logBuilder = logBuilder;

    public class Registration :
        RegisterStep
    {
        public Registration(LogBuilder logBuilder) :
            base(
                stepId: Name,
                behavior: typeof(InjectIncomingLogicalBehavior),
                description: "Injects a logger into the incoming logical context",
                factoryMethod: _ => new InjectIncomingLogicalBehavior(logBuilder)
            )
        {
        }
    }

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
            new("IncomingMessageType", typeName.MessageTypeName),
            new("IncomingMessageTypeLong", typeName.LongName)
        };
        var logger = logBuilder.GetLogger(typeName.MessageTypeName);

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