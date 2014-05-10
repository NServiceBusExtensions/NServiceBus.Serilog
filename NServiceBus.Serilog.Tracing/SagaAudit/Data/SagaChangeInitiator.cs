using System;

namespace NServiceBus.Serilog.Tracing
{
    class SagaChangeInitiator
    {
        public string InitiatingMessageId;
        public string MessageType;
        public bool IsSagaTimeoutMessage;
        public DateTime TimeSent;
        public string OriginatingMachine;
        public string OriginatingEndpoint;
        public string Intent;
    }
}