using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using NServiceBus.Pipeline;
using Serilog;
using Serilog.Events;
using Serilog.Parsing;

class ReceiveMessageBehavior : Behavior<IIncomingLogicalMessageContext>
{
    MessageTemplate messageTemplate;
    ILogger logger;

    public ReceiveMessageBehavior(LogBuilder logBuilder)
    {
        var templateParser = new MessageTemplateParser();
        messageTemplate = templateParser.Parse("Receive message {MessageType} {MessageId}.");
        logger = logBuilder.GetLogger("NServiceBus.Serilog.MessageReceived");
    }

    public class Registration : RegisterStep
    {
        public Registration()
            : base("SerilogReceiveMessage", typeof(ReceiveMessageBehavior), "Logs incoming messages")
        {
            InsertBefore("MutateIncomingMessages");
        }
    }

    public override Task Invoke(IIncomingLogicalMessageContext context, Func<Task> next)
    {
        var message = context.Message;
        var properties = new List<LogEventProperty>
        {
            new LogEventProperty("MessageType", new ScalarValue(message.MessageType))
        };

        if (logger.BindProperty("Message", message.Instance, out var messageProperty))
        {
            properties.Add(messageProperty);
        }

        if (logger.BindProperty("MessageId", context.MessageId, out var messageId))
        {
            properties.Add(messageId);
        }

        properties.AddRange(logger.BuildHeaders(context.Headers));
        logger.WriteInfo(messageTemplate, properties);
        return next();
    }
}