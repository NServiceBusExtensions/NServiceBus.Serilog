﻿class OutgoingBehavior :
    Behavior<IOutgoingLogicalMessageContext>
{
    LogBuilder logBuilder;

    OutgoingBehavior(LogBuilder logBuilder) =>
        this.logBuilder = logBuilder;

    public override async Task Invoke(IOutgoingLogicalMessageContext context, Func<Task> next)
    {
        var headers = context.Headers;

        var bag = context.Extensions;

        var type = context.Message.Instance.GetType();
        var messageTypeName = TypeNameConverter.GetName(type)
            .MessageTypeName;
        if (!bag.TryGet<ILogger>(out var logger))
        {
            // if it a raw session send (ie no handler/saga, there will be no current logger)
            logger = logBuilder.GetLogger(messageTypeName);
        }

        var properties = new List<PropertyEnricher>
        {
            new("OutgoingMessageId", context.MessageId),
            new("OutgoingMessageType", messageTypeName)
        };

        if (headers.TryGetValue(Headers.CorrelationId, out var correlationId))
        {
            properties.Add(new("CorrelationId", correlationId));
        }

        if (headers.TryGetValue(Headers.ConversationId, out var conversationId))
        {
            properties.Add(new("ConversationId", conversationId));
        }

        var forContext = logger.ForContext(properties);
        try
        {
            bag.Set("SerilogOutgoingLogger", forContext);
            await next();
        }
        finally
        {
            bag.Remove("SerilogOutgoingLogger");
        }
    }

    public class Registration(LogBuilder logBuilder) :
        RegisterStep(
            stepId: $"Serilog{nameof(OutgoingBehavior)}",
            behavior: typeof(OutgoingBehavior),
            description: "Injects a logger into the outgoing context",
            factoryMethod: _ => new OutgoingBehavior(logBuilder));
}