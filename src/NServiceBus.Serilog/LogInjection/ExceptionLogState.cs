using System;
using System.Collections.Generic;

#pragma warning disable 1591

[Serializable]
class ExceptionLogState
{
    public readonly string ProcessingEndpoint;
    public readonly string? CorrelationId;
    public readonly string? ConversationId;
    public string? HandlerType;
    public object? IncomingMessage;
    public readonly IReadOnlyDictionary<string, string> IncomingHeaders;

    public ExceptionLogState(string processingEndpoint, IReadOnlyDictionary<string, string> incomingHeaders, string? correlationId, string? conversationId)
    {
        Guard.AgainstNull(processingEndpoint, nameof(processingEndpoint));
        ProcessingEndpoint = processingEndpoint;
        IncomingHeaders = incomingHeaders;
        CorrelationId = correlationId;
        ConversationId = conversationId;
    }
}