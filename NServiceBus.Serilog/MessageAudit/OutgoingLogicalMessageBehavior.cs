using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using NServiceBus;
using NServiceBus.Pipeline;
using Serilog;
using Serilog.Core.Enrichers;
using Serilog.Events;
using Serilog.Parsing;

class OutgoingLogicalMessageBehavior : Behavior<IOutgoingLogicalMessageContext>
{
    LogBuilder logBuilder;
    MessageTemplate messageTemplate;

    public OutgoingLogicalMessageBehavior(LogBuilder logBuilder)
    {
        this.logBuilder = logBuilder;
        var templateParser = new MessageTemplateParser();
        messageTemplate = templateParser.Parse("Sent message {MessageType} {MessageId}.");
    }

    public override Task Invoke(IOutgoingLogicalMessageContext context, Func<Task> next)
    {
        var headers = context.Headers;
        var message = context.Message.Instance;
        var forContext = InjectLoggerIntoContext(context, message, headers);

        LogMessage(context, forContext, message);
        return next();
    }

    ILogger InjectLoggerIntoContext(IOutgoingContext context, object message, Dictionary<string, string> headers)
    {
        var messageType = message.GetType();

        var logger = logBuilder.GetLogger(messageType.FullName);

        var properties = new List<PropertyEnricher>
        {
            new PropertyEnricher("MessageId", context.MessageId),
            new PropertyEnricher("MessageType", messageType.AssemblyQualifiedName),
            new PropertyEnricher("CorrelationId", headers[Headers.CorrelationId]),
            new PropertyEnricher("ConversationId", headers[Headers.ConversationId])
        };

        var forContext = logger.ForContext(properties);
        context.Extensions.Set(forContext);
        return forContext;
    }

    void LogMessage(IOutgoingContext context, ILogger forContext, object message)
    {
        var logProperties = new List<LogEventProperty>();

        if (forContext.BindProperty("Message", message, out var messageProperty))
        {
            logProperties.Add(messageProperty);
        }

        logProperties.AddRange(forContext.BuildHeaders(context.Headers));
        forContext.WriteInfo(messageTemplate, logProperties);
    }

    public class Registration : RegisterStep
    {
        public Registration(LogBuilder logBuilder)
            : base(
                stepId: $"Serilog{nameof(OutgoingLogicalMessageBehavior)}",
                behavior: typeof(OutgoingLogicalMessageBehavior),
                description: "Logs outgoing messages",
                factoryMethod: builder => new OutgoingLogicalMessageBehavior(logBuilder))
        {
        }
    }
}