static class SerilogExtensions
{
    public static bool BindProperty(
        this ILogger logger,
        string name,
        object value,
        [NotNullWhen(true)] out LogEventProperty? property) =>
        logger.BindProperty(name, value, true, out property);

    public static void WriteInfo(
        this ILogger logger,
        MessageTemplate messageTemplate,
        IEnumerable<LogEventProperty> properties)
    {
        var logEvent = new LogEvent(
            timestamp: DateTimeOffset.Now,
            level: LogEventLevel.Information,
            exception: null,
            messageTemplate: messageTemplate,
            properties: properties);
        logger.Write(logEvent);
    }

    public static string ToLogString(this DateTimeOffset date) =>
        date.ToString("yyyy-MM-ddTHH:mm:ss.fffzz");

    public static LogEventProperty BuildDictionaryProperty(string name, IReadOnlyDictionary<string, string> otherHeaders) =>
        new(
            name,
            new DictionaryValue(
                otherHeaders.Select(_ =>
                    new KeyValuePair<ScalarValue, LogEventPropertyValue>(
                        new(_.Key),
                        new ScalarValue(_.Value)))));
}