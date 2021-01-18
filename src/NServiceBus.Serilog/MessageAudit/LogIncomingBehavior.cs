using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using NServiceBus;
using NServiceBus.Pipeline;
using Serilog.Events;
using Serilog.Parsing;

class LogIncomingBehavior :
    Behavior<IIncomingLogicalMessageContext>
{
    bool useFullTypeName;
    ConvertHeader convertHeader;
    static MessageTemplate messageTemplate;

    public LogIncomingBehavior(bool useFullTypeName, ConvertHeader convertHeader)
    {
        this.useFullTypeName = useFullTypeName;
        this.convertHeader = convertHeader;
    }

    static LogIncomingBehavior()
    {
        MessageTemplateParser templateParser = new();
        messageTemplate = templateParser.Parse("Receive message {IncomingMessageType} {IncomingMessageId}.");
    }

    public class Registration :
        RegisterStep
    {
        public Registration(bool useFullTypeName, ConvertHeader convertHeader) :
            base(
                stepId: $"Serilog{nameof(LogIncomingBehavior)}",
                behavior: typeof(LogIncomingBehavior),
                description: "Logs incoming messages",
                factoryMethod: _ => new LogIncomingBehavior(useFullTypeName, convertHeader))
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

        properties.AddRange(logger.BuildHeaders(useFullTypeName, context.Headers, convertHeader));
        logger.WriteInfo(messageTemplate, properties);
        return next();
    }
}