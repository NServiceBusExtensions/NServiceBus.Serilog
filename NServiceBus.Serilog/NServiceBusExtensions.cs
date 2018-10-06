using System;
using NServiceBus;
using NServiceBus.Pipeline;

// ReSharper disable CSharpWarnings::CS0618
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
}