[DebuggerDisplay("Source {StringSourceContext}")]
public class LogEventEx
{
    public readonly MessageTemplate MessageTemplate;
    public readonly LogEventLevel Level;
    public readonly IReadOnlyDictionary<string, LogEventPropertyValue> Properties;
    public readonly Exception? Exception;

    public LogEventEx(MessageTemplate messageTemplate, LogEventLevel level, IReadOnlyDictionary<string, LogEventPropertyValue> properties, Exception? exception)
    {
        MessageTemplate = messageTemplate;
        Level = level;
        Properties = properties;
        Exception = exception;
    }

    public string? StringSourceContext
    {
        get
        {
            if (Properties.TryGetValue("SourceContext", out var sourceContext))
            {
                if (sourceContext is ScalarValue {Value: string temp})
                {
                    return temp;
                }
            }

            return null;
        }
    }
}