#pragma warning disable 1591

[Serializable]
class ExceptionLogState
{
    public readonly string ProcessingEndpoint;
    public readonly string? CorrelationId;
    public readonly string? ConversationId;
    public object? IncomingMessage;
    public readonly IReadOnlyDictionary<string, string> IncomingHeaders;

    public ExceptionLogState(string processingEndpoint, IReadOnlyDictionary<string, string> incomingHeaders, string? correlationId, string? conversationId)
    {
        ProcessingEndpoint = processingEndpoint;
        IncomingHeaders = incomingHeaders;
        CorrelationId = correlationId;
        ConversationId = conversationId;
    }
}