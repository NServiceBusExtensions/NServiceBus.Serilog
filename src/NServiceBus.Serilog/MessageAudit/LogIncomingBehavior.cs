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
        messageTemplate = templateParser.Parse("Receive message {IncomingMessageType} {IncomingMessageId} ({ElapsedTime:N3}s).");
    }

    public static string Name = $"Serilog{nameof(LogIncomingBehavior)}";

    public class Registration :
        RegisterStep
    {
        public Registration(ConvertHeader convertHeader) :
            base(
                stepId: Name,
                behavior: typeof(LogIncomingBehavior),
                description: "Logs incoming messages",
                factoryMethod: _ => new LogIncomingBehavior(convertHeader))
        {
            InsertBefore("MutateIncomingMessages");
            InsertAfter(InjectIncomingLogicalBehavior.Name);
        }
    }

    public override async Task Invoke(IIncomingLogicalMessageContext context, Func<Task> next)
    {
        var message = context.Message;
        var logger = context.Logger();
        var startTime = DateTimeOffset.UtcNow;
        await next();
        var finishTime = DateTimeOffset.UtcNow;
        var properties = new List<LogEventProperty>
        {
            new("StartTime", new ScalarValue(startTime)),
            new("FinishTime", new ScalarValue(finishTime)),
            new("ElapsedTime", new ScalarValue((finishTime - startTime).TotalSeconds))
        };

        if (logger.BindProperty("IncomingMessage", message.Instance, out var property))
        {
            properties.Add(property);
        }

        properties.AddRange(HeaderAppender.BuildHeaders(context.Headers, convertHeader));
        logger.WriteInfo(messageTemplate, properties);
    }
}