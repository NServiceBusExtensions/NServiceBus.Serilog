using NServiceBus;
using NServiceBus.Pipeline;
using NServiceBus.Routing;

static class NServiceBusExtensions
{
    public static string OriginatingMachine(this IInvokeHandlerContext context)
    {
        if (context.Headers.TryGetValue(Headers.OriginatingMachine, out var intent))
        {
            return intent;
        }

        return string.Empty;
    }

    public static string OriginatingEndpoint(this IInvokeHandlerContext context)
    {
        if (context.Headers.TryGetValue(Headers.OriginatingEndpoint, out var endpoint))
        {
            return endpoint;
        }

        return string.Empty;
    }

    public static string MessageIntent(this IInvokeHandlerContext context)
    {
        return MessageIntent(context.Headers);
    }

    public static string HandlerType(this IInvokeHandlerContext context)
    {
        return context.MessageHandler.HandlerType.FullName;
    }

    public static Type MessageType(this IInvokeHandlerContext context)
    {
        return context.MessageMetadata.MessageType;
    }

    public static string MessageIntent(this IOutgoingLogicalMessageContext context)
    {
        return MessageIntent(context.Headers);
    }

    static string MessageIntent(Dictionary<string, string> headers)
    {
        if (headers.TryGetValue(Headers.MessageIntent, out var intent))
        {
            return intent;
        }

        return "Send";
    }

    static Dictionary<string, string> emptyDictionary = new();

    public static List<string> UnicastAddresses(this IOutgoingPhysicalMessageContext context)
    {
        return context.RoutingStrategies
            .OfType<UnicastRoutingStrategy>()
            .Select(x => ((UnicastAddressTag) x.Apply(emptyDictionary)).Destination)
            .ToList();
    }

    public static DateTime TimeSent(this IInvokeHandlerContext context)
    {
        return DateTimeExtensions.ToUtcDateTime(context.Headers[Headers.TimeSent]);
    }

    public static bool IsTimeoutMessage(this IInvokeHandlerContext context)
    {
        if (context.Headers.TryGetValue(Headers.IsSagaTimeoutMessage, out var isTimeoutString))
        {
            return string.Equals(isTimeoutString, "true", StringComparison.OrdinalIgnoreCase);
        }

        return false;
    }

    public static string? GetDestinationForUnicastMessages(this IOutgoingLogicalMessageContext context)
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