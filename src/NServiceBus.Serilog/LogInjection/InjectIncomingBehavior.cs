using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using NServiceBus;
using NServiceBus.Pipeline;
using NServiceBus.Serilog;
using Serilog.Core.Enrichers;

class InjectIncomingBehavior :
    Behavior<IIncomingPhysicalMessageContext>
{
    LogBuilder logBuilder;
    string endpoint;

    InjectIncomingBehavior(LogBuilder logBuilder, string endpoint)
    {
        this.logBuilder = logBuilder;
        this.endpoint = endpoint;
    }

    public class Registration :
        RegisterStep
    {
        public Registration(LogBuilder logBuilder, string endpoint) :
            base(
                stepId: $"Serilog{nameof(InjectIncomingBehavior)}",
                behavior: typeof(InjectIncomingBehavior),
                description: "Injects a logger into the incoming context",
                factoryMethod: _ => new InjectIncomingBehavior(logBuilder, endpoint)
            )
        {
        }
    }

    public override async Task Invoke(IIncomingPhysicalMessageContext context, Func<Task> next)
    {
        string messageTypeName;
        var headers = context.MessageHeaders;
        if (headers.TryGetValue(Headers.EnclosedMessageTypes, out var messageType))
        {
            messageTypeName = TypeNameConverter.GetName(messageType);
        }
        else
        {
            messageTypeName = "UnknownMessageType";
        }

        var logger = logBuilder.GetLogger(messageTypeName);
        List<PropertyEnricher> properties = new()
        {
            new("IncomingMessageId", context.MessageId),
            new("IncomingMessageType", messageTypeName)
        };


        if (headers.TryGetValue(Headers.CorrelationId, out var correlationId))
        {
            properties.Add(new PropertyEnricher("CorrelationId", correlationId));
        }

        if (headers.TryGetValue(Headers.ConversationId, out var conversationId))
        {
            properties.Add(new PropertyEnricher("ConversationId", conversationId));
        }

        ExceptionLogState exceptionLogState = new
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