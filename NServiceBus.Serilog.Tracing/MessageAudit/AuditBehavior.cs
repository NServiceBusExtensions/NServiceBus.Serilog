using System;
using NServiceBus.Audit;
using NServiceBus.Pipeline;
using NServiceBus.Pipeline.Contexts;

namespace NServiceBus.Serilog.Tracing
{
    // ReSharper disable CSharpWarnings::CS0618
    class AuditBehavior : IBehavior<ReceivePhysicalMessageContext>
    {
        public MessageAuditer MessageAuditer { get; set; }

        public void Invoke(ReceivePhysicalMessageContext context, Action next)
        {
            next();
            MessageAuditer.ForwardMessageToAuditQueue(context.PhysicalMessage);
        }
    }
}
