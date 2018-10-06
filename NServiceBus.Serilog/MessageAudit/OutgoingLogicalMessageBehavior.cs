using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using NServiceBus.Pipeline;
using Serilog;
using Serilog.Events;
using Serilog.Parsing;

class OutgoingLogicalMessageBehavior : Behavior<IOutgoingLogicalMessageContext>
{
    ILogger logger;
    MessageTemplate messageTemplate;

    public OutgoingLogicalMessageBehavior(LogBuilder logBuilder)
    {
        var templateParser = new MessageTemplateParser();
        logger = logBuilder.GetLogger("NServiceBus.Serilog." + nameof(OutgoingLogicalMessageBehavior));
        messageTemplate = templateParser.Parse("Sent message {MessageType} {MessageId}.");
    }

    public override Task Invoke(IOutgoingLogicalMessageContext context, Func<Task> next)
    {
        var message = context.Message;
        var properties = new List<LogEventProperty>
        {
            new LogEventProperty("MessageType", new ScalarValue(message.MessageType))
        };

        if (logger.BindProperty("MessageId", context.MessageId, out var messageId))
        {
            properties.Add(messageId);
        }

        if (logger.BindProperty("Message", message.Instance, out var messageProperty))
        {
            properties.Add(messageProperty);
        }


        properties.AddRange(logger.BuildHeaders(context.Headers));
        logger.WriteInfo(messageTemplate, properties);
        return next();
    }

    public class Registration : RegisterStep
    {
        public Registration(LogBuilder logBuilder)
            : base(
                stepId: "Serilog" + nameof(OutgoingLogicalMessageBehavior),
                behavior: typeof(OutgoingLogicalMessageBehavior),
                description: "Logs outgoing messages",
                factoryMethod: builder => new OutgoingLogicalMessageBehavior(logBuilder))
        {
        }
    }
}