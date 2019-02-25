using System;
using System.Collections.Generic;

#pragma warning disable 1591

namespace NServiceBus.Serilog
{
    [Serializable]
    public class ExceptionLogState
    {
        public string ProcessingEndpoint;
        public string MessageId;
        public string MessageType;
        public string CorrelationId;
        public string ConversationId;
        public string HandlerType;
        public object Message;
        public IReadOnlyDictionary<string, string> Headers;
    }
}