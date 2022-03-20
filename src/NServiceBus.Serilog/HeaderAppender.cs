static class HeaderAppender
{
    static List<string> excludeHeaders = new()
    {
        Headers.EnclosedMessageTypes,
        Headers.ProcessingEndpoint,
        Headers.ContentType,
        Headers.CorrelationId,
        Headers.ConversationId,
        Headers.NServiceBusVersion,
        Headers.MessageId
    };

    public static IEnumerable<LogEventProperty> BuildHeaders(this ILogger logger, IReadOnlyDictionary<string, string> headers, ConvertHeader convertHeader)
    {
        var otherHeaders = new Dictionary<string, string>();
        foreach (var header in headers
            .Where(x => !excludeHeaders.Contains(x.Key))
            .OrderBy(x => x.Key))
        {
            var key = header.Key;
            var value = header.Value;

            var converted = convertHeader(key, value);
            if (converted is not null)
            {
                yield return converted;
                continue;
            }

            if (key == Headers.TimeSent)
            {
                yield return new(key.Substring(12), new ScalarValue(DateTimeExtensions.ToUtcDateTime(value)));
                continue;
            }

            if (key == Headers.OriginatingSagaType)
            {
                value = TypeNameConverter.GetName(value);
                yield return new(nameof(Headers.OriginatingSagaType), new ScalarValue(value));
                continue;
            }

            if (key.StartsWith("NServiceBus."))
            {
                yield return new(key.Substring(12), new ScalarValue(value));
                continue;
            }

            if (key == Headers.OriginatingHostId)
            {
                yield return new(nameof(Headers.OriginatingHostId), new ScalarValue(value));
                continue;
            }

            if (key == Headers.HostDisplayName)
            {
                yield return new(nameof(Headers.HostDisplayName), new ScalarValue(value));
                continue;
            }

            if (key == Headers.HostId)
            {
                yield return new(nameof(Headers.HostId), new ScalarValue(value));
                continue;
            }

            otherHeaders.Add(key, value);
        }

        if (otherHeaders.Count > 0)
        {
            if (logger.BindProperty("OtherHeaders", otherHeaders, out var property))
            {
                yield return property;
            }
        }
    }
}