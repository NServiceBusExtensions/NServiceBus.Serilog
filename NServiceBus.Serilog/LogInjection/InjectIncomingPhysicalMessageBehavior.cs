using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using NServiceBus;
using NServiceBus.Pipeline;
using Serilog.Core.Enrichers;

class InjectIncomingPhysicalMessageBehavior : Behavior<IIncomingPhysicalMessageContext>
{
    LogBuilder logBuilder;
    string endpoint;

    public InjectIncomingPhysicalMessageBehavior(LogBuilder logBuilder, string endpoint)
    {
        this.logBuilder = logBuilder;
        this.endpoint = endpoint;
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
        string messageTypeName;
        var headers = context.MessageHeaders;
        if (headers.TryGetValue(Headers.EnclosedMessageTypes, out var messageType))
        {
            messageTypeName = TypeHelper.GetShortTypeName(messageType);
        }
        else
        {
            messageTypeName = "UnknownMessageType";
        }

        var logger = logBuilder.GetLogger(messageTypeName);
        var properties = new List<PropertyEnricher>
        {
            new PropertyEnricher("MessageId", context.MessageId),
            new PropertyEnricher("MessageType", messageTypeName)
        };

        var exceptionLogState = new ExceptionLogState
        {
            Endpoint = endpoint,
            MessageId = context.MessageId,
            MessageType = messageTypeName,
        };
        if (headers.TryGetValue(Headers.CorrelationId, out var correlationId))
        {
            exceptionLogState.CorrelationId = correlationId;
            properties.Add(new PropertyEnricher("CorrelationId", correlationId));
        }

        if (headers.TryGetValue(Headers.ConversationId, out var conversationId))
        {
            exceptionLogState.ConversationId = conversationId;
            properties.Add(new PropertyEnricher("ConversationId", conversationId));
        }

        var loggerForContext = logger.ForContext(properties);
        context.Extensions.Set(loggerForContext);


        try
        {
            await next().ConfigureAwait(false);
        }
        catch (Exception exception)
        {
            exception.Data.Add("ExceptionLogState", exceptionLogState);
            throw;
        }
    }
}