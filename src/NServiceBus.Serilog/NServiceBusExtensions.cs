﻿static class NServiceBusExtensions
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

    public static string MessageIntent(this IInvokeHandlerContext context) =>
        MessageIntent(context.Headers);

    public static string HandlerType(this IInvokeHandlerContext context) =>
        context.MessageHandler.HandlerType.FullName!;

    public static Type MessageType(this IInvokeHandlerContext context) =>
        context.MessageMetadata.MessageType;

    public static string MessageIntent(this IOutgoingLogicalMessageContext context) =>
        MessageIntent(context.Headers);

    static string MessageIntent(Dictionary<string, string> headers) =>
        headers.GetValueOrDefault(Headers.MessageIntent, "Send");

    static Dictionary<string, string> emptyDictionary = [];

    public static List<string> UnicastAddresses(this IOutgoingPhysicalMessageContext context) =>
        context
            .RoutingStrategies
            .OfType<UnicastRoutingStrategy>()
            .Select(_ => ((UnicastAddressTag) _.Apply(emptyDictionary)).Destination)
            .ToList();

    public static DateTimeOffset TimeSent(this IInvokeHandlerContext context) =>
        DateTimeOffsetHelper.ToDateTimeOffset(context.Headers[Headers.TimeSent]);

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
        foreach (var strategy in context
                     .RoutingStrategies)
        {
            if (strategy is UnicastRoutingStrategy unicastRouting)
            {
                var tag = (UnicastAddressTag) unicastRouting.Apply(context.Headers);
                return tag.Destination;
            }
        }

        return null;
    }
}