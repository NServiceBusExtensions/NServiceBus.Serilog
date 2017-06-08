namespace NServiceBus.Serilog.Tracing
{
    class SagaChangeOutput
    {
        public string MessageType { get; set; }
        public string Destination { get; set; }
        public string ResultingMessageId { get; set; }
        public string MessageIntent { get; set; }
    }
}