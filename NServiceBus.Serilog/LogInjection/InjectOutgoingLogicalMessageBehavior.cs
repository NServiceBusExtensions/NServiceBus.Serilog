using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using NServiceBus.Pipeline;
using Serilog.Core.Enrichers;

class InjectOutgoingLogicalMessageBehavior : Behavior<IOutgoingLogicalMessageContext>
{
    LogBuilder logBuilder;

    public InjectOutgoingLogicalMessageBehavior(LogBuilder logBuilder)
    {
        this.logBuilder = logBuilder;
    }

    public override Task Invoke(IOutgoingLogicalMessageContext context, Func<Task> next)
    {
        var headers = context.Headers;

        var messageTypeName = context.Message.Instance.GetType().FullName;
        var logger = logBuilder.GetLogger(messageTypeName);

        var properties = new List<PropertyEnricher>
        {
            new PropertyEnricher("MessageId", context.MessageId),
            new PropertyEnricher("MessageType", messageTypeName),
        };

        HeaderPromote.PromoteCorrAndConv(headers, properties);
        var forContext = logger.ForContext(properties);
        context.Extensions.Set(forContext);

        return next();
    }

    public class Registration : RegisterStep
    {
        public Registration(LogBuilder logBuilder)
            : base(
                stepId: $"Serilog{nameof(InjectOutgoingLogicalMessageBehavior)}",
                behavior: typeof(InjectOutgoingLogicalMessageBehavior),
                description: "Injects a logger into the outgoing context",
                factoryMethod: builder => new InjectOutgoingLogicalMessageBehavior(logBuilder))
        {
        }
    }
}