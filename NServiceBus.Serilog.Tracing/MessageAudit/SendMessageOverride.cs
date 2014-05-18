using NServiceBus.Pipeline;
using NServiceBus.Pipeline.Contexts;
using NServiceBus.Unicast.Behaviors;

namespace NServiceBus.Serilog.Tracing
{
    // ReSharper disable CSharpWarnings::CS0618
    class SendMessageOverride : PipelineOverride
    {
        public override void Override(BehaviorList<SendPhysicalMessageContext> behaviorList)
        {
            if (!TracingLog.IsEnabled())
            {
                return;
            }
            behaviorList.InsertAfter<DispatchMessageToTransportBehavior, SendMessageBehavior>();
        }
    }
}