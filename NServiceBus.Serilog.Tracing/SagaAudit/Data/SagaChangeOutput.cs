namespace NServiceBus.Serilog.Tracing
{
    class SagaChangeOutput
    {
        public string MessageType;
        public string Destination;
        public string ResultingMessageId;
        public string MessageIntent;
    }
}