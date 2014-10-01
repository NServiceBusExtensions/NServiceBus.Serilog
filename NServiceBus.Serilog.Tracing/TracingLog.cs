namespace NServiceBus.Serilog.Tracing
{
    using global::Serilog;
    using NServiceBus.Features;
    using NServiceBus.Pipeline;

    public class TracingLog:Feature
    {

        public TracingLog()
        {
            EnableByDefault();
        }

        protected override void Setup(FeatureConfigurationContext context)
        {
            context.Pipeline.Register<CaptureSagaStateRegistration>();
            context.Pipeline.Register<CaptureSagaResultingMessageRegistration>();
            context.Pipeline.Register<ReceiveMessageRegistration>();
            context.Pipeline.Register<SendMessageRegistration>();
            ILogger logger;
            if (!context.Settings.TryGetSerilogTracingTarget(out logger))
            {
                logger = Log.Logger;
            }
            var logBuilder = new LogBuilder(logger, context.Settings.EndpointName());
            context.Container.ConfigureComponent(() => logBuilder, DependencyLifecycle.SingleInstance);
        }

        class CaptureSagaStateRegistration : RegisterStep
        {
            public CaptureSagaStateRegistration()
                : base("SerilogCaptureSagaState", typeof(CaptureSagaStateBehavior), "Records saga state changes")
            {
                InsertBefore(WellKnownStep.InvokeSaga);
            }
        }

        class CaptureSagaResultingMessageRegistration : RegisterStep
        {
            public CaptureSagaResultingMessageRegistration()
                : base("SerilogReportSagaStateChanges", typeof(CaptureSagaResultingMessagesBehavior), "Reports the saga state changes to Serilog")
            {
                InsertAfter(WellKnownStep.InvokeSaga);
            }
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