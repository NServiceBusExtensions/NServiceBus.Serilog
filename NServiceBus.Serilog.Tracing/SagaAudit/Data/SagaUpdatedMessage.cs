using System;
using System.Collections.Generic;

namespace NServiceBus.Serilog.Tracing
{
    class SagaUpdatedMessage
    {
        public SagaUpdatedMessage()
        {
            ResultingMessages = new List<SagaChangeOutput>();
        }

        public Guid SagaId;
        public List<SagaChangeOutput> ResultingMessages;
        public bool IsNew;
        public bool IsCompleted;
        public DateTime StartTime;
        public DateTime FinishTime;
        public string SagaType;
    }
}