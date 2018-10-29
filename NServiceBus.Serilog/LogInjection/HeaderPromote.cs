using System.Collections.Generic;
using NServiceBus;
using Serilog.Core.Enrichers;

static class HeaderPromote
{
    public static void PromoteCorrAndConv(IReadOnlyDictionary<string, string> headers, List<PropertyEnricher> properties)
    {
        if (headers.TryGetValue(Headers.CorrelationId, out var correlationId))
        {
            properties.Add(new PropertyEnricher("CorrelationId", correlationId));
        }

        if (headers.TryGetValue(Headers.ConversationId, out var conversationId))
        {
            properties.Add(new PropertyEnricher("ConversationId", conversationId));
        }
    }
}