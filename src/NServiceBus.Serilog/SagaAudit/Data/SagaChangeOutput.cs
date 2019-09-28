namespace NServiceBus.Serilog
{
    class SagaChangeOutput
    {
        public SagaChangeOutput(string resultingMessageId, string messageType, string? destination, string messageIntent)
        {
            ResultingMessageId = resultingMessageId;
            MessageType = messageType;
            Destination = destination;
            MessageIntent = messageIntent;
        }

        public string MessageType { get; }
        public string? Destination { get; }
        public string ResultingMessageId { get; }
        public string MessageIntent { get; }
    }
}