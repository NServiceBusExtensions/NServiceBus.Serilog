class InjectIncomingBehavior :
    Behavior<IIncomingPhysicalMessageContext>
{
    LogBuilder logBuilder;
    string endpoint;

    public InjectIncomingBehavior(LogBuilder logBuilder, string endpoint)
    {
        this.logBuilder = logBuilder;
        this.endpoint = endpoint;
    }

    public class Registration :
        RegisterStep
    {
        public Registration(LogBuilder logBuilder, string endpoint) :
            base(
                stepId: $"Serilog{nameof(InjectIncomingBehavior)}",
                behavior: typeof(InjectIncomingBehavior),
                description: "Injects a logger into the incoming context",
                factoryMethod: _ => new InjectIncomingBehavior(logBuilder, endpoint)
            )
        {
        }
    }

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
            if (split.Length == 1)
            {
                var longName = split[0];
                longName = longName.Replace(", Culture=neutral", "").Replace(", PublicKeyToken=null", "");
                var messageTypeName = TypeNameConverter.GetName(longName);
                properties.Add(new("IncomingMessageType", messageTypeName));
                properties.Add(new("IncomingMessageTypeLong", longName));
                logger = logBuilder.GetLogger(messageTypeName);
            }
            else
            {
                var names = split.Select(TypeNameConverter.GetName).ToList();
                properties.Add(new("IncomingMessageTypes", names));
                properties.Add(new("IncomingMessageTypesLong", split));
                var messageTypeName = string.Join(";", names);
                logger = logBuilder.GetLogger(messageTypeName);
            }
        }
        else
        {
            properties.Add(new("IncomingMessageType", "UnknownMessageType"));

            logger = logBuilder.GetLogger("UnknownMessageType");
        }

        if (headers.TryGetValue(Headers.CorrelationId, out var correlationId))
        {
            properties.Add(new("CorrelationId", correlationId));
        }

        if (headers.TryGetValue(Headers.ConversationId, out var conversationId))
        {
            properties.Add(new("ConversationId", conversationId));
        }

        ExceptionLogState exceptionLogState = new
        (
            processingEndpoint: endpoint,
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