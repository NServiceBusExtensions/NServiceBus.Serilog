using System;
#pragma warning disable 1591

namespace NServiceBus.Serilog
{
    [Serializable]
    public class ExceptionLogState
    {
        public string Endpoint;
        public string MessageId;
        public string MessageType;
        public string CorrelationId;
        public string ConversationId;
    }
}