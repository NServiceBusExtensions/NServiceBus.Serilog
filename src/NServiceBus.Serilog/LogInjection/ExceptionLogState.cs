using System;
using System.Collections.Generic;

#pragma warning disable 1591

namespace NServiceBus.Serilog
{
    [Serializable]
    public class ExceptionLogState
    {
        public string ProcessingEndpoint;
        public string IncomingMessageId;
        public string IncomingMessageType;
        public string CorrelationId;
        public string ConversationId;
        public string HandlerType;
        public object IncomingMessage;
        public IReadOnlyDictionary<string, string> IncomingHeaders;
    }
}