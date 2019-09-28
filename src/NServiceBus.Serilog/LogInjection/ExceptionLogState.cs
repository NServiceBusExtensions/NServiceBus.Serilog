using System;
using System.Collections.Generic;

#pragma warning disable 1591

namespace NServiceBus.Serilog
{
    [Serializable]
    public class ExceptionLogState
    {
        public readonly string ProcessingEndpoint;
        public readonly string IncomingMessageId;
        public readonly string IncomingMessageType;
        public readonly string CorrelationId;
        public readonly string ConversationId;
        public string? HandlerType;
        public object? IncomingMessage;
        public readonly IReadOnlyDictionary<string, string> IncomingHeaders;

        public ExceptionLogState(string processingEndpoint, string incomingMessageId, string incomingMessageType, IReadOnlyDictionary<string, string> incomingHeaders, string correlationId, string conversationId)
        {
            ProcessingEndpoint = processingEndpoint;
            IncomingMessageId = incomingMessageId;
            IncomingMessageType = incomingMessageType;
            IncomingHeaders = incomingHeaders;
            CorrelationId = correlationId;
            ConversationId = conversationId;
        }
    }
}