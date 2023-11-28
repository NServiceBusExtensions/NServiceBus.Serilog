[DebuggerDisplay("Source {StringSourceContext}")]
public class LogEventEx(MessageTemplate messageTemplate, LogEventLevel level, IReadOnlyDictionary<string, LogEventPropertyValue> properties, Exception? exception)
{
    public readonly MessageTemplate MessageTemplate = messageTemplate;
    public readonly LogEventLevel Level = level;
    public readonly IReadOnlyDictionary<string, LogEventPropertyValue> Properties = properties;
    public readonly Exception? Exception = exception;

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