using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using NServiceBus;
using NServiceBus.Pipeline;
using Serilog;
using Serilog.Core.Enrichers;

class InjectIncomingPhysicalMessageBehavior : Behavior<IIncomingPhysicalMessageContext>
{
    LogBuilder logBuilder;
    ILogger loggerForPipeline;

    public InjectIncomingPhysicalMessageBehavior(LogBuilder logBuilder)
    {
        this.logBuilder = logBuilder;
        loggerForPipeline = logBuilder.GetLogger("NServiceBus.Serilog.Pipeline");
    }

    public class Registration : RegisterStep
    {
        public Registration(LogBuilder logBuilder)
            : base(
                stepId: $"Serilog{nameof(InjectIncomingPhysicalMessageBehavior)}",
                behavior: typeof(InjectIncomingPhysicalMessageBehavior),
                description: "Injects a logger into the incoming context",
                factoryMethod: builder => new InjectIncomingPhysicalMessageBehavior(logBuilder)
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

        HeaderPromote.PromoteCorrAndConv(headers, properties);

        var forContext = logger.ForContext(properties);
        context.Extensions.Set(forContext);
        return next();
    }
}