#pragma warning disable 1591

[Serializable]
class ExceptionLogState(string processingEndpoint, IReadOnlyDictionary<string, string> incomingHeaders, string? correlationId, string? conversationId)
{
    public readonly string ProcessingEndpoint = processingEndpoint;
    public readonly string? CorrelationId = correlationId;
    public readonly string? ConversationId = conversationId;
    public object? IncomingMessage;
    public readonly IReadOnlyDictionary<string, string> IncomingHeaders = incomingHeaders;
}