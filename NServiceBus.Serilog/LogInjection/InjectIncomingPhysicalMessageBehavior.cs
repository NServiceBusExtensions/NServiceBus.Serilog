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
    string endpoint;
    ILogger loggerForPipeline;

    public InjectIncomingPhysicalMessageBehavior(LogBuilder logBuilder, string endpoint)
    {
        this.logBuilder = logBuilder;
        this.endpoint = endpoint;
        loggerForPipeline = logBuilder.GetLogger("NServiceBus.Serilog.Pipeline");
    }

    public class Registration : RegisterStep
    {
        public Registration(LogBuilder logBuilder, string endpoint)
            : base(
                stepId: $"Serilog{nameof(InjectIncomingPhysicalMessageBehavior)}",
                behavior: typeof(InjectIncomingPhysicalMessageBehavior),
                description: "Injects a logger into the incoming context",
                factoryMethod: builder => new InjectIncomingPhysicalMessageBehavior(logBuilder, endpoint)
            )
        {
        }
    }

    public override async Task Invoke(IIncomingPhysicalMessageContext context, Func<Task> next)
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

        var loggerForContext = logger.ForContext(properties);
        context.Extensions.Set(loggerForContext);

        try
        {
            await next().ConfigureAwait(false);
        }
        catch (Exception exception)
        {
            var data = exception.Data;
            data.Add("ProcessingEndpoint", endpoint);
            foreach (var header in context.MessageHeaders)
            {
                var key = header.Key;
                if (key.StartsWith("NServiceBus."))
                {
                    data.Add(key.Substring(12), header.Value);
                    continue;
                }

                if (key == Headers.OriginatingHostId)
                {
                    data.Add(nameof(Headers.OriginatingHostId), header.Value);
                    continue;
                }
                if (key == Headers.HostDisplayName)
                {
                    data.Add(nameof(Headers.HostDisplayName), header.Value);
                    continue;
                }
                if (key == Headers.HostId)
                {
                    data.Add(nameof(Headers.HostId), header.Value);
                    continue;
                }


                data.Add(key, header.Value);
            }

            throw;
        }
    }
}