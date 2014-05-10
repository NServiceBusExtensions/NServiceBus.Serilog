using System;

namespace NServiceBus.Serilog.Tracing
{
    class SagaChangeOutput
    {
        public string MessageType;
        public DateTime TimeSent;
        public DateTime? DeliveryAt;
        public TimeSpan? DeliveryDelay;
        public string Destination;
        public string ResultingMessageId;
        public string Intent;
    }
}