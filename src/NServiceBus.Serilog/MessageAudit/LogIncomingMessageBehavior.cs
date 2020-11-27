using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using NServiceBus;
using NServiceBus.Pipeline;
using Serilog.Events;
using Serilog.Parsing;

class LogIncomingMessageBehavior :
    Behavior<IIncomingLogicalMessageContext>
{
    static MessageTemplate messageTemplate;

    static LogIncomingMessageBehavior()
    {
        MessageTemplateParser templateParser = new();
        messageTemplate = templateParser.Parse("Receive message {IncomingMessageTypeShort} {IncomingMessageId}.");
    }

    public class Registration :
        RegisterStep
    {
        public Registration() :
            base(
                stepId: $"Serilog{nameof(LogIncomingMessageBehavior)}",
                behavior: typeof(LogIncomingMessageBehavior),
                description: "Logs incoming messages")
        {
            InsertBefore("MutateIncomingMessages");
        }
    }

    public override Task Invoke(IIncomingLogicalMessageContext context, Func<Task> next)
    {
        var message = context.Message;
        List<LogEventProperty> properties = new();

        var logger = context.Logger();
        if (logger.BindProperty("IncomingMessage", message.Instance, out var property))
        {
            properties.Add(property);
        }

        properties.AddRange(logger.BuildHeaders(context.Headers));
        logger.WriteInfo(messageTemplate, properties);
        return next();
    }
}