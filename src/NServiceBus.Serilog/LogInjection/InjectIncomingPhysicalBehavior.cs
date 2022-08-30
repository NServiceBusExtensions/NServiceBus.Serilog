﻿class InjectIncomingPhysicalBehavior :
    Behavior<IIncomingPhysicalMessageContext>
{
    LogBuilder logBuilder;
    string endpoint;

    public InjectIncomingPhysicalBehavior(LogBuilder logBuilder, string endpoint)
    {
        this.logBuilder = logBuilder;
        this.endpoint = endpoint;
    }

    public class Registration :
        RegisterStep
    {
        public Registration(LogBuilder logBuilder, string endpoint) :
            base(
                stepId: $"Serilog{nameof(InjectIncomingPhysicalBehavior)}",
                behavior: typeof(InjectIncomingPhysicalBehavior),
                description: "Injects a logger into the incoming physical context",
                factoryMethod: _ => new InjectIncomingPhysicalBehavior(logBuilder, endpoint)
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
            var names = split.Select(TypeNameConverter.GetName).ToList();
            properties.Add(new("IncomingMessageTypes", names));
            properties.Add(new("IncomingMessageTypesLong", split));
            var messageTypeName = string.Join(";", names);
            logger = logBuilder.GetLogger(messageTypeName);
        }
        else
        {
            properties.Add(new("IncomingMessageTypes", Array.Empty<string>()));
            properties.Add(new("IncomingMessageTypesLong", Array.Empty<string>()));

            logger = logBuilder.GetLogger("UnknownMessageTypes");
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