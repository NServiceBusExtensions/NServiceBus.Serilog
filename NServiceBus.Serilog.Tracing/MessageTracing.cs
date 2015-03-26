using NServiceBus.Features;
using NServiceBus.Pipeline;

namespace NServiceBus.Serilog.Tracing {
    public class MessageTracing : Feature
    {
        public MessageTracing() 
        {
            EnableByDefault();
            DependsOn<TracingLog>();
        }

        protected override void Setup(FeatureConfigurationContext context) 
        {
            context.Pipeline.Register<ReceiveMessageRegistration>();
            context.Pipeline.Register<SendMessageRegistration>();
        }

        class ReceiveMessageRegistration : RegisterStep 
        {
            public ReceiveMessageRegistration()
                : base("SerilogReceiveMessage", typeof(ReceiveMessageBehavior), "Logs incoming messages") 
            {
                InsertBefore(WellKnownStep.MutateIncomingMessages);
            }
        }

        class SendMessageRegistration : RegisterStep 
        {
            public SendMessageRegistration()
                : base("SerilogSendMessage", typeof(SendMessageBehavior), "Logs outgoing messages") 
            {
                InsertAfter(WellKnownStep.DispatchMessageToTransport);
            }
        }
    }
}
