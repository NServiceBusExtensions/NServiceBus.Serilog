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
        string shortTypeName = null;
        if (headers.TryGetValue(Headers.EnclosedMessageTypes, out var messageType))
        {
            shortTypeName = TypeHelper.GetShortTypeName(messageType);
            logger = logBuilder.GetLogger(shortTypeName);
            properties.Add(new PropertyEnricher("MessageType", shortTypeName));
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
            AddContextToData(exception, shortTypeName, headers);
            throw;
        }
    }

    void AddContextToData(Exception exception, string shortTypeName, IReadOnlyDictionary<string, string> headers)
    {
        var data = exception.Data;
        data.Add("ProcessingEndpoint", endpoint);
        if (shortTypeName != null)
        {
            data.Add("MessageType", shortTypeName);
        }

        foreach (var header in headers)
        {
            var key = header.Key;
            var value = header.Value;
            if (key == Headers.NServiceBusVersion)
            {
                data.Add(nameof(Headers.NServiceBusVersion), value);
                continue;
            }

            if (key == Headers.EnclosedMessageTypes)
            {
                continue;
            }

            if (key.StartsWith("NServiceBus."))
            {
                data.Add(key.Substring(12), value);
                continue;
            }

            if (key == Headers.OriginatingHostId)
            {
                data.Add(nameof(Headers.OriginatingHostId), value);
                continue;
            }

            if (key == Headers.HostDisplayName)
            {
                data.Add(nameof(Headers.HostDisplayName), value);
                continue;
            }

            if (key == Headers.HostId)
            {
                data.Add(nameof(Headers.HostId), value);
                continue;
            }

            data.Add(key, value);
        }
    }
}