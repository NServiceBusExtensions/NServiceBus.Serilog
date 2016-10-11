using System.Collections.Generic;
using System.Linq;
using Serilog;
using Serilog.Events;

namespace NServiceBus.Serilog.Tracing
{
    static class HeaderAppender
    {
        static List<string> excludeHeaders = new List<string>
        {
            Headers.EnclosedMessageTypes, 
            Headers.ProcessingEndpoint, 
            "NServiceBus.Version"
        };
        
        public static IEnumerable<LogEventProperty> BuildHeaders(this ILogger logger, Dictionary<string, string> headers)
        {
            var otherHeaders = new Dictionary<string,string>();
            foreach (var header in headers
                .Where(x => x.Key.StartsWith("NServiceBus.") && !excludeHeaders.Contains(x.Key))
                .OrderBy(x=>x.Key))
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
                yield return logger.BindProperty("OtherHeaders", otherHeaders);
            }
        }
    }
}