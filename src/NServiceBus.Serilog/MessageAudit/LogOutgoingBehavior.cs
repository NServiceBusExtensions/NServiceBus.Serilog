class LogOutgoingBehavior :
    Behavior<IOutgoingPhysicalMessageContext>
{
    ConvertHeader convertHeader;
    MessageTemplate messageTemplate;

    LogOutgoingBehavior(ConvertHeader convertHeader)
    {
        this.convertHeader = convertHeader;
        var templateParser = new MessageTemplateParser();
        messageTemplate = templateParser.Parse("Sent message {OutgoingMessageType} {OutgoingMessageId}.");
    }

    public override Task Invoke(IOutgoingPhysicalMessageContext context, Func<Task> next)
    {
        var message = context.Extensions.Get<OutgoingLogicalMessage>().Instance;
        LogMessage(context, context.Logger(), message);
        return next();
    }

    void LogMessage(IOutgoingPhysicalMessageContext context, ILogger forContext, object message)
    {
        var properties = new List<LogEventProperty>();

        if (forContext.BindProperty("OutgoingMessage", message, out var messageProperty))
        {
            properties.Add(messageProperty);
        }

        var addresses = context.UnicastAddresses();
        if (addresses.Count > 0)
        {
            var sequence = new SequenceValue(addresses.Select(x => new ScalarValue(x)));
            properties.Add(new("UnicastRoutes", sequence));
        }

        properties.AddRange(forContext.BuildHeaders(context.Headers, convertHeader));
        forContext.WriteInfo(messageTemplate, properties);
    }

    public class Registration :
        RegisterStep
    {
        public Registration(ConvertHeader convertHeader) :
            base(
                stepId: $"Serilog{nameof(LogOutgoingBehavior)}",
                behavior: typeof(LogOutgoingBehavior),
                description: "Logs outgoing messages",
                factoryMethod: _ => new LogOutgoingBehavior(convertHeader))
        {
        }
    }
}