using System;
using System.Collections.Generic;
using System.Linq;
using NServiceBus;
using NServiceBus.Pipeline;
using NServiceBus.Routing;

static class NServiceBusExtensions
{
    public static string MessageIntent(this IInvokeHandlerContext logicalMessage)
    {
        var headers = logicalMessage.Headers;
        if (headers.TryGetValue(Headers.MessageIntent, out var intent))
        {
            return intent;
        }

        return "Send";
    }

    static Dictionary<string, string> emptyDictionary = new Dictionary<string, string>();

    public static List<string> UnicastAddresses(this IOutgoingLogicalMessageContext context)
    {
        return context.RoutingStrategies
            .OfType<UnicastRoutingStrategy>()
            .Select(x => ((UnicastAddressTag) x.Apply(emptyDictionary)).Destination)
            .ToList();
    }

    public static DateTime TimeSent(this IInvokeHandlerContext logicalMessage)
    {
        return DateTimeExtensions.ToUtcDateTime(logicalMessage.Headers[Headers.TimeSent]);
    }

    public static bool IsTimeoutMessage(this IInvokeHandlerContext message)
    {
        if (message.Headers.TryGetValue(Headers.IsSagaTimeoutMessage, out var isTimeoutString))
        {
            return string.Equals(isTimeoutString, "true", StringComparison.OrdinalIgnoreCase);
        }

        return false;
    }

    public static string GetDestinationForUnicastMessages(this IOutgoingLogicalMessageContext context)
    {
        var sendAddressTags = context.RoutingStrategies
            .OfType<UnicastRoutingStrategy>()
            .Select(urs => urs.Apply(context.Headers))
            .Cast<UnicastAddressTag>().ToList();
        if (sendAddressTags.Count != 1)
        {
            return null;
        }

        return sendAddressTags.First().Destination;
    }
}