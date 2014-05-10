using NServiceBus.Features;
using NServiceBus.Pipeline;
using NServiceBus.Pipeline.Contexts;

namespace NServiceBus.Serilog.Tracing
{
    // ReSharper disable CSharpWarnings::CS0618
    class MessageAuditingOverride : PipelineOverride
    {
        public override void Override(BehaviorList<ReceivePhysicalMessageContext> behaviorList)
        {
            if (!Feature.IsEnabled<SerilogMessageAudit>())
            {
                return;
            }

            behaviorList.InsertBefore<Audit.AuditBehavior, AuditBehavior>();
        }
    }
}