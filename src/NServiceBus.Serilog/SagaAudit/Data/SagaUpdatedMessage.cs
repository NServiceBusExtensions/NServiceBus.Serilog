using System;
using System.Collections.Generic;

namespace NServiceBus.Serilog
{
    class SagaUpdatedMessage
    {
        public Guid SagaId { get; set; }
        public List<SagaChangeOutput> ResultingMessages { get; set; } = new List<SagaChangeOutput>();
        public bool IsNew { get; set; }
        public bool IsCompleted { get; set; }
        public DateTimeOffset StartTime { get; set; }
        public DateTimeOffset FinishTime { get; set; }
        public string SagaType { get; set; }
    }
}