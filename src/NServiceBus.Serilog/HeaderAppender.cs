using System.Collections.Generic;
using System.Linq;
using NServiceBus;
using Serilog;
using Serilog.Events;

static class HeaderAppender
{
    internal static List<string> excludeHeaders = new
    {
        Headers.EnclosedMessageTypes,
        Headers.ProcessingEndpoint,
        Headers.ContentType,
        Headers.CorrelationId,
        Headers.ConversationId,
        Headers.NServiceBusVersion,
        Headers.MessageId
    };

    public static IEnumerable<LogEventProperty> BuildHeaders(this ILogger logger, IReadOnlyDictionary<string, string> headers)
    {
        var otherHeaders = new Dictionary<string, string>();
        foreach (var header in headers
            .Where(x => !excludeHeaders.Contains(x.Key))
            .OrderBy(x => x.Key))
        {
            var key = header.Key;
            var value = header.Value;
            if (key == Headers.TimeSent)
            {
                yield return new LogEventProperty(key.Substring(12), new ScalarValue(DateTimeExtensions.ToUtcDateTime(value)));
                continue;
            }
            if (key.StartsWith("NServiceBus."))
            {
                yield return new LogEventProperty(key.Substring(12), new ScalarValue(value));
                continue;
            }

            if (key == Headers.OriginatingHostId)
            {
                yield return new LogEventProperty(nameof(Headers.OriginatingHostId), new ScalarValue(value));
                continue;
            }

            if (key == Headers.HostDisplayName)
            {
                yield return new LogEventProperty(nameof(Headers.HostDisplayName), new ScalarValue(value));
                continue;
            }

            if (key == Headers.HostId)
            {
                yield return new LogEventProperty(nameof(Headers.HostId), new ScalarValue(value));
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