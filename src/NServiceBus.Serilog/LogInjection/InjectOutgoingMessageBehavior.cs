using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using NServiceBus;
using NServiceBus.Pipeline;
using Serilog;
using Serilog.Core.Enrichers;

class InjectOutgoingMessageBehavior :
    Behavior<IOutgoingLogicalMessageContext>
{
    LogBuilder logBuilder;

    public InjectOutgoingMessageBehavior(LogBuilder logBuilder)
    {
        this.logBuilder = logBuilder;
    }

    public override async Task Invoke(IOutgoingLogicalMessageContext context, Func<Task> next)
    {
        var headers = context.Headers;

        var bag = context.Extensions;
        var messageTypeName = context.Message.Instance.GetType().FullName;
        if (!bag.TryGet<ILogger>(out var logger))
        {
            // if it a raw session send (ie no handler/saga, there will be no current logger)
            logger = logBuilder.GetLogger(messageTypeName);
        }

        var properties = new List<PropertyEnricher>
        {
            new PropertyEnricher("OutgoingMessageId", context.MessageId),
            new PropertyEnricher("OutgoingMessageType", messageTypeName),
        };

        if (headers.TryGetValue(Headers.CorrelationId, out var correlationId))
        {
            properties.Add(new PropertyEnricher("CorrelationId", correlationId));
        }

        if (headers.TryGetValue(Headers.ConversationId, out var conversationId))
        {
            properties.Add(new PropertyEnricher("ConversationId", conversationId));
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

    public class Registration :
        RegisterStep
    {
        public Registration(LogBuilder logBuilder)
            : base(
                stepId: $"Serilog{nameof(InjectOutgoingMessageBehavior)}",
                behavior: typeof(InjectOutgoingMessageBehavior),
                description: "Injects a logger into the outgoing context",
                factoryMethod: builder => new InjectOutgoingMessageBehavior(logBuilder))
        {
        }
    }
}