using NServiceBus.Pipeline;
using NServiceBus.Pipeline.Contexts;
using NServiceBus.Sagas;

namespace NServiceBus.Serilog.Tracing
{
    // ReSharper disable CSharpWarnings::CS0618
    class SagaStateAuditingOverride : PipelineOverride
    {
        public override void Override(BehaviorList<HandlerInvocationContext> behaviorList)
        {
            if (!TracingLog.IsEnabled())
            {
                return;
            }
            behaviorList.InsertBefore<SagaPersistenceBehavior, CaptureSagaStateBehavior>();
        }
        public override void Override(BehaviorList<SendPhysicalMessageContext> behaviorList)
        {
            if (!TracingLog.IsEnabled())
            {
                return;
            }
            behaviorList.Add<CaptureSagaResultingMessagesBehavior>();
        }
    }
}