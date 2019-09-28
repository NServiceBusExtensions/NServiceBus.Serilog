using System;

namespace NServiceBus.Serilog
{
    class SagaChangeInitiator
    {
        public SagaChangeInitiator(bool isSagaTimeoutMessage, string initiatingMessageId, string originatingMachine, string originatingEndpoint, string messageType, DateTime timeSent, string intent)
        {
            IsSagaTimeoutMessage = isSagaTimeoutMessage;
            InitiatingMessageId = initiatingMessageId;
            OriginatingMachine = originatingMachine;
            OriginatingEndpoint = originatingEndpoint;
            MessageType = messageType;
            TimeSent = timeSent;
            Intent = intent;
        }

        public string InitiatingMessageId { get; }
        public string MessageType { get; }
        public bool IsSagaTimeoutMessage { get; }
        public DateTime TimeSent { get; }
        public string OriginatingMachine { get; }
        public string OriginatingEndpoint { get; }
        public string Intent { get; }
    }
}