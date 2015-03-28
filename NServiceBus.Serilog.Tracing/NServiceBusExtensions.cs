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
            if (logicalMessage.IsControlMessage())
            {
                return "ControlMessage";
            }
            var type = logicalMessage.MessageType;
            if (type.Namespace == null)
            {
                return type.Name;
            }
            return string.Format("{0}.{1}", type.Namespace, type.Name);
        }

        public static DateTime TimeSent(this LogicalMessage logicalMessage)
        {
            return DateTimeExtensions.ToUtcDateTime(logicalMessage.Headers[Headers.TimeSent]);
        }


        public static bool IsControlMessage(this LogicalMessage transportMessage)
        {
            if (transportMessage.Headers == null)
            {
                return false;
            }
            string isControlMessage;
            if (transportMessage.Headers.TryGetValue(Headers.ControlMessageHeader, out isControlMessage))
            {
                return isControlMessage == "true";
            }
            return false;
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