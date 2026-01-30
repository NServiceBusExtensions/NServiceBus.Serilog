class IncomingPhysicalBehavior(LogBuilder builder, string endpoint) :
    Behavior<IIncomingPhysicalMessageContext>
{
    public class Registration(LogBuilder logBuilder, string endpoint) :
        RegisterStep(
            stepId: $"Serilog{nameof(IncomingPhysicalBehavior)}",
            behavior: typeof(IncomingPhysicalBehavior),
            description: nameof(IncomingPhysicalBehavior),
            factoryMethod: _ => new IncomingPhysicalBehavior(logBuilder, endpoint));

    static PropertyEnricher emptyIncomingMessageTypes = new("IncomingMessageTypes", Array.Empty<string>());
    static PropertyEnricher emptyIncomingMessageTypesLong = new("IncomingMessageTypesLong", Array.Empty<string>());
    LogEventProperty processingEndpoint = new("ProcessingEndpoint", new ScalarValue(endpoint));

    public override async Task Invoke(IIncomingPhysicalMessageContext context, Func<Task> next)
    {
        var properties = new List<PropertyEnricher>
        {
            new("IncomingMessageId", context.MessageId)
        };

        ILogger logger;
        var headers = context.MessageHeaders;
        if (headers.TryGetValue(Headers.EnclosedMessageTypes, out var enclosedMessageTypes))
        {
            var split = enclosedMessageTypes.Split(';');
            var names = split
                .Select(TypeNameConverter.GetName)
                .ToList();
            properties.Add(new("IncomingMessageTypes", names));
            properties.Add(new("IncomingMessageTypesLong", split));
            var messageTypeName = string.Join(';', names);
            logger = builder.GetLogger(messageTypeName);
        }
        else
        {
            properties.Add(emptyIncomingMessageTypes);
            properties.Add(emptyIncomingMessageTypesLong);

            logger = builder.GetLogger("UnknownMessageTypes");
        }

        if (headers.TryGetValue(Headers.CorrelationId, out var correlationId))
        {
            properties.Add(new("CorrelationId", correlationId));
        }

        if (headers.TryGetValue(Headers.ConversationId, out var conversationId))
        {
            properties.Add(new("ConversationId", conversationId));
        }

        var exceptionLogState = new ExceptionLogState
        (
            processingEndpoint: processingEndpoint,
            incomingHeaders: context.MessageHeaders,
            correlationId: correlationId,
            conversationId: conversationId
        );

        var loggerForContext = logger.ForContext(properties);
        context.Extensions.Set(exceptionLogState);
        context.Extensions.Set(loggerForContext);

        try
        {
            await next();
        }
        catch (Exception exception)
        {
            var data = exception.Data;
            if (!data.Contains("ExceptionLogState"))
            {
                data.Add("ExceptionLogState", exceptionLogState);
            }

            throw;
        }
    }
}