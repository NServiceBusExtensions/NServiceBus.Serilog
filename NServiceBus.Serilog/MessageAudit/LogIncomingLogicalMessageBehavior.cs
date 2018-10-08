using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using NServiceBus;
using NServiceBus.Pipeline;
using Serilog.Events;
using Serilog.Parsing;

class LogIncomingLogicalMessageBehavior : Behavior<IIncomingLogicalMessageContext>
{
    MessageTemplate messageTemplate;

    public LogIncomingLogicalMessageBehavior()
    {
        var templateParser = new MessageTemplateParser();
        messageTemplate = templateParser.Parse("Receive message {MessageType} {MessageId}.");
    }

    public class Registration : RegisterStep
    {
        public Registration()
            : base(
                stepId: $"Serilog{nameof(LogIncomingLogicalMessageBehavior)}",
                behavior: typeof(LogIncomingLogicalMessageBehavior),
                description: "Logs incoming messages",
                factoryMethod: builder => new LogIncomingLogicalMessageBehavior()
                )
        {
            InsertBefore("MutateIncomingMessages");
        }
    }

    public override Task Invoke(IIncomingLogicalMessageContext context, Func<Task> next)
    {
        var message = context.Message;
        var properties = new List<LogEventProperty>();

        var logger = context.Logger();
        if (logger.BindProperty("Message", message.Instance, out var messageProperty))
        {
            properties.Add(messageProperty);
        }

        properties.AddRange(logger.BuildHeaders(context.Headers));
        logger.WriteInfo(messageTemplate, properties);
        return next();
    }
}