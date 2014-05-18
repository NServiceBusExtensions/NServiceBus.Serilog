using NServiceBus.Pipeline;
using NServiceBus.Pipeline.Contexts;
using NServiceBus.Pipeline.MessageMutator;

namespace NServiceBus.Serilog.Tracing
{
    // ReSharper disable CSharpWarnings::CS0618
    class ReceiveMessageOverride : PipelineOverride
    {
        public override void Override(BehaviorList<ReceiveLogicalMessageContext> behaviorList)
        {
            if (!TracingLog.IsEnabled())
            {
                return;
            }
            behaviorList.InsertBefore<ApplyIncomingMessageMutatorsBehavior, ReceiveMessageBehavior>();
        }
    }
}