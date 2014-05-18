using System;
using NServiceBus.Pipeline.Contexts;
using NServiceBus.Unicast.Messages;

namespace NServiceBus.Serilog.Tracing
{
    // ReSharper disable CSharpWarnings::CS0618
    static class NServiceBusExtensions
    {

        public static string MessageIntent(this HandlerInvocationContext context)
        {
            var headers = context.LogicalMessage.Headers;
            return headers.ContainsKey(Headers.MessageIntent) ? headers[Headers.MessageIntent] : "Send"; // Just in case the received message is from an early version that does not have intent, should be a rare occasion.
        }

        public static DateTime TimeSent(this SendPhysicalMessageContext context)
        {
            return DateTimeExtensions.ToUtcDateTime(context.MessageToSend.Headers[Headers.TimeSent]);
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

        public static DateTime TimeSent(this HandlerInvocationContext context)
        {
            return DateTimeExtensions.ToUtcDateTime(context.LogicalMessage.Headers[Headers.TimeSent]);
        }

        public static string Destination(this SendPhysicalMessageContext context)
        {
            // Destination can be null for publish events
            if (context.SendOptions.Destination != null)
            {
                return context.SendOptions.Destination.ToString();
            }
            return null;
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