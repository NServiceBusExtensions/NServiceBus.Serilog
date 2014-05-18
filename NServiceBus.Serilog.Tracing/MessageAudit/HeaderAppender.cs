using System.Collections.Generic;
using System.Linq;
using Serilog;

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
        
        public static ILogger AddHeaders(this ILogger forContext, Dictionary<string, string> headers)
        {
            var otherHeaders = new Dictionary<string,string>();
            foreach (var header in headers
                .Where(x => x.Key.StartsWith("NServiceBus.") && !excludeHeaders.Contains(x.Key))
                .OrderBy(x=>x.Key))
            {
                if (header.Key.StartsWith("$.diagnostics.") || header.Key.StartsWith("NServiceBus."))
                {
                    if (!excludeHeaders.Contains(header.Key))
                    {
                        forContext = forContext.ForContext(header.Key.Replace("NServiceBus.", ""), header.Value);
                    }
                    continue;
                }
                otherHeaders.Add(header.Key, header.Value);
            }
            if (otherHeaders.Count > 0)
            {
                forContext.ForContext("OtherHeaders", otherHeaders, true);
            }
            return forContext;
        }
    }
}