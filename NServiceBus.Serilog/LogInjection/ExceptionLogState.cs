using System;

[Serializable]
internal class ExceptionLogState
{
    public string Endpoint;
    public string MessageId;
    public string MessageType;
    public string CorrelationId;
    public string ConversationId;
}