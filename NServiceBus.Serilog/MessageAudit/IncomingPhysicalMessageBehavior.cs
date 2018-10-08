using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using NServiceBus;
using NServiceBus.Pipeline;
using Serilog;
using Serilog.Core.Enrichers;

class TypeHelper
{
    public static string GetShortTypeName(string messageType) => messageType.Substring(0, messageType.IndexOf(","));
}

class IncomingPhysicalMessageBehavior : Behavior<IIncomingPhysicalMessageContext>
{
    LogBuilder logBuilder;
    ILogger loggerForPipeline;

    public IncomingPhysicalMessageBehavior(LogBuilder logBuilder)
    {
        this.logBuilder = logBuilder;
        loggerForPipeline = logBuilder.GetLogger("NServiceBus.Serilog.Pipeline");
    }

    public class Registration : RegisterStep
    {
        public Registration(LogBuilder logBuilder)
            : base(
                stepId: $"Serilog{nameof(IncomingPhysicalMessageBehavior)}",
                behavior: typeof(IncomingPhysicalMessageBehavior),
                description: "Logs incoming messages",
                factoryMethod: builder => new IncomingPhysicalMessageBehavior(logBuilder)
            )
        {
        }
    }

    public override Task Invoke(IIncomingPhysicalMessageContext context, Func<Task> next)
    {
        var properties = new List<PropertyEnricher>
        {
            new PropertyEnricher("MessageId", context.MessageId)
        };
        ILogger logger;
        var headers = context.MessageHeaders;
        if (headers.TryGetValue(Headers.EnclosedMessageTypes, out var messageType))
        {
            logger = logBuilder.GetLogger(TypeHelper.GetShortTypeName(messageType));
            properties.Add(new PropertyEnricher("MessageType", messageType));
        }
        else
        {
            logger = loggerForPipeline;
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
        context.Extensions.Set(forContext);
        return next();
    }
}