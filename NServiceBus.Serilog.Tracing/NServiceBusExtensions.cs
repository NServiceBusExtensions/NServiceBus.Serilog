using System;
using NServiceBus.Unicast.Messages;

namespace NServiceBus.Serilog.Tracing
{
    // ReSharper disable CSharpWarnings::CS0618
    static class NServiceBusExtensions
    {

        public static string MessageIntent(this LogicalMessage logicalMessage)
        {
            var headers = logicalMessage.Headers;
            string intent;
            if (headers.TryGetValue(Headers.MessageIntent, out intent))
            {
                return intent;
            }
            return "Send";
        }

        public static string MessageTypeName(this LogicalMessage logicalMessage)
        {
            var type = logicalMessage.MessageType;
            if (type.Namespace != null)
            {
                return String.Format("{0}.{1}", type.Namespace, type.Name);
            }
            return type.Name;
        }

        public static DateTime TimeSent(this LogicalMessage logicalMessage)
        {
            return DateTimeExtensions.ToUtcDateTime(logicalMessage.Headers[Headers.TimeSent]);
        }

        public static bool IsTimeoutMessage(this LogicalMessage message)
        {
            string isTimeoutString;
            if (message.Headers.TryGetValue(Headers.IsSagaTimeoutMessage, out isTimeoutString))
            {
                return isTimeoutString == "true";
            }
            return false;
        }
    }
}