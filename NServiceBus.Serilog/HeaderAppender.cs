using System.Collections.Generic;
using System.Linq;
using NServiceBus;
using Serilog;
using Serilog.Events;

static class HeaderAppender
{
    internal static List<string> excludeHeaders = new List<string>
    {
        Headers.EnclosedMessageTypes,
        Headers.ProcessingEndpoint,
        Headers.ContentType,
        Headers.CorrelationId,
        Headers.ConversationId,
        "NServiceBus.Version",
        Headers.MessageId
    };

    public static IEnumerable<LogEventProperty> BuildHeaders(this ILogger logger, IReadOnlyDictionary<string, string> headers)
    {
        var otherHeaders = new Dictionary<string, string>();
        foreach (var header in headers
            .Where(x => x.Key.StartsWith("NServiceBus.") && !excludeHeaders.Contains(x.Key))
            .OrderBy(x => x.Key))
        {
            var key = header.Key;
            if (key.StartsWith("$.diagnostics.") || key.StartsWith("NServiceBus."))
            {
                if (!excludeHeaders.Contains(key))
                {
                    var replace = key.Replace("NServiceBus.", "");
                    yield return new LogEventProperty(replace, new ScalarValue(header.Value));
                }

                continue;
            }

            otherHeaders.Add(key, header.Value);
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