static class HeaderAppender
{
    static List<string> excludeHeaders =
    [
        Headers.EnclosedMessageTypes,
        Headers.ProcessingEndpoint,
        Headers.ContentType,
        Headers.CorrelationId,
        Headers.ConversationId,
        Headers.NServiceBusVersion,
        Headers.MessageId
    ];

    public static IEnumerable<LogEventProperty> BuildHeaders(IReadOnlyDictionary<string, string> headers, ConvertHeader? convertHeader)
    {
        var otherHeaders = new Dictionary<string, string>();
        foreach (var header in headers
                     .Where(_ => !excludeHeaders.Contains(_.Key))
                     .OrderBy(_ => _.Key))
        {
            var key = header.Key;
            var value = header.Value;

            var converted = convertHeader?.Invoke(key, value);
            if (converted != null)
            {
                yield return converted;
                continue;
            }

            if (key.StartsWith("NServiceBus."))
            {
                var name = key[12..];
                if (key == Headers.TimeSent)
                {
                    yield return new(name, new ScalarValue(DateTimeOffsetHelper.ToDateTimeOffset(value)));
                    continue;
                }

                if (key == Headers.OriginatingSagaType)
                {
                    value = TypeNameConverter.GetName(value);
                    yield return new(nameof(Headers.OriginatingSagaType), new ScalarValue(value));
                    continue;
                }

                yield return new(name, new ScalarValue(value));
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
            yield return SerilogExtensions.BuildDictionaryProperty("OtherHeaders", otherHeaders);
        }
    }
}