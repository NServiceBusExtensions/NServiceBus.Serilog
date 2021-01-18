using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using NServiceBus;
using NServiceBus.Pipeline;
using Serilog;
using Serilog.Core.Enrichers;

class InjectOutgoingBehavior :
    Behavior<IOutgoingLogicalMessageContext>
{
    LogBuilder logBuilder;
    bool useFullTypeName;

    public InjectOutgoingBehavior(LogBuilder logBuilder, bool useFullTypeName)
    {
        this.logBuilder = logBuilder;
        this.useFullTypeName = useFullTypeName;
    }

    public override async Task Invoke(IOutgoingLogicalMessageContext context, Func<Task> next)
    {
        var headers = context.Headers;

        var bag = context.Extensions;

        var type = context.Message.Instance.GetType();
        var messageTypeName = type.Name;
        if (!bag.TryGet<ILogger>(out var logger))
        {
            // if it a raw session send (ie no handler/saga, there will be no current logger)
            logger = logBuilder.GetLogger(messageTypeName);
        }

        var properties = new List<PropertyEnricher>
        {
            new("OutgoingMessageId", context.MessageId),
        };
        if (useFullTypeName)
        {
            properties.Add(new("OutgoingMessageType", type.FullName));
        }
        else
        {
            properties.Add(new("OutgoingMessageType", messageTypeName));
        }

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
        public Registration(LogBuilder logBuilder, bool useFullTypeName) :
            base(
                stepId: $"Serilog{nameof(InjectOutgoingBehavior)}",
                behavior: typeof(InjectOutgoingBehavior),
                description: "Injects a logger into the outgoing context",
                factoryMethod: _ => new InjectOutgoingBehavior(logBuilder, useFullTypeName))
        {
        }
    }
}