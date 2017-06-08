using System;
using NServiceBus.Pipeline;

namespace NServiceBus.Serilog.Tracing
{
    // ReSharper disable CSharpWarnings::CS0618
    static class NServiceBusExtensions
    {

        public static string MessageIntent(this IInvokeHandlerContext logicalMessage)
        {
            var headers = logicalMessage.Headers;
            string intent;
            if (headers.TryGetValue(Headers.MessageIntent, out intent))
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
            string isTimeoutString;
            if (message.Headers.TryGetValue(Headers.IsSagaTimeoutMessage, out isTimeoutString))
            {
                return string.Equals(isTimeoutString, "true", StringComparison.OrdinalIgnoreCase);
            }
            return false;
        }
    }
}