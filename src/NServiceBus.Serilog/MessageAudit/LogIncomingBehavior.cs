class LogIncomingBehavior :
    Behavior<IIncomingLogicalMessageContext>
{
    ConvertHeader convertHeader;
    static MessageTemplate messageTemplate;

    LogIncomingBehavior(ConvertHeader convertHeader) =>
        this.convertHeader = convertHeader;

    static LogIncomingBehavior()
    {
        var templateParser = new MessageTemplateParser();
        messageTemplate = templateParser.Parse("Receive message {IncomingMessageType} {IncomingMessageId}.");
    }

    public class Registration :
        RegisterStep
    {
        public Registration(ConvertHeader convertHeader) :
            base(
                stepId: $"Serilog{nameof(LogIncomingBehavior)}",
                behavior: typeof(LogIncomingBehavior),
                description: "Logs incoming messages",
                factoryMethod: _ => new LogIncomingBehavior(convertHeader)) =>
            InsertBefore("MutateIncomingMessages");
    }

    public override Task Invoke(IIncomingLogicalMessageContext context, Func<Task> next)
    {
        var message = context.Message;
        var properties = new List<LogEventProperty>();

        var logger = context.Logger();
        if (logger.BindProperty("IncomingMessage", message.Instance, out var property))
        {
            properties.Add(property);
        }

        properties.AddRange(logger.BuildHeaders(context.Headers, convertHeader));
        logger.WriteInfo(messageTemplate, properties);
        return next();
    }
}