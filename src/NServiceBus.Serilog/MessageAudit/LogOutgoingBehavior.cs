﻿class LogOutgoingBehavior :
    Behavior<IOutgoingPhysicalMessageContext>
{
    ConvertHeader convertHeader;
    static MessageTemplate messageTemplate;

    LogOutgoingBehavior(ConvertHeader convertHeader) =>
        this.convertHeader = convertHeader;

    static LogOutgoingBehavior()
    {
        var templateParser = new MessageTemplateParser();
        messageTemplate = templateParser.Parse("Sent message {OutgoingMessageType} {OutgoingMessageId}.");
    }

    public override Task Invoke(IOutgoingPhysicalMessageContext context, Func<Task> next)
    {
        var message = context.Extensions
            .Get<OutgoingLogicalMessage>()
            .Instance;
        LogMessage(context, message);
        return next();
    }

    void LogMessage(IOutgoingPhysicalMessageContext context, object message)
    {
        //TODO: try to make instance based on the type
        var logger = Log.ForContext<LogOutgoingBehavior>();
        var properties = new List<LogEventProperty>();

        if (logger.BindProperty("OutgoingMessage", message, out var messageProperty))
        {
            properties.Add(messageProperty);
        }

        var addresses = context.UnicastAddresses();
        if (addresses.Count > 0)
        {
            if (addresses.Count == 1)
            {
                properties.Add(new("Route", new ScalarValue(addresses[0])));
            }
            else
            {
                var sequence = new SequenceValue(addresses.Select(_ => new ScalarValue(_)));
                properties.Add(new("Routes", sequence));
            }
        }

        properties.AddRange(HeaderAppender.BuildHeaders(context.Headers, convertHeader));
        logger.WriteInfo(messageTemplate, properties);
    }

    public class Registration(ConvertHeader convertHeader) :
        RegisterStep(
            stepId: $"Serilog{nameof(LogOutgoingBehavior)}",
            behavior: typeof(LogOutgoingBehavior),
            description: "Logs outgoing messages",
            factoryMethod: _ => new LogOutgoingBehavior(convertHeader));
}