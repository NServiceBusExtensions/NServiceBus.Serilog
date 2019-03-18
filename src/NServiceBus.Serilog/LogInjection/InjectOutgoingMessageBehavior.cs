using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using NServiceBus;
using NServiceBus.Pipeline;
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

        var messageTypeName = context.Message.Instance.GetType().FullName;
        var logger = logBuilder.GetLogger(messageTypeName);

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
            context.Extensions.Set("SerilogOutgoingLogger", forContext);
            await next();
        }
        finally
        {
            context.Extensions.Remove("SerilogOutgoingLogger");
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