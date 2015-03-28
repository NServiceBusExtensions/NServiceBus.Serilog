using NServiceBus.Features;
using NServiceBus.Pipeline;
using Serilog;

namespace NServiceBus.Serilog.Tracing
{
    /// <summary>
    /// Enabled tracing of messages to Serilog.
    /// </summary>
    public class TracingLog:Feature
    {
        /// <summary>
        /// Initializes a new instance of <see cref="TracingLog"/>.
        /// </summary>
        public TracingLog()
        {
            EnableByDefault();
        }

        /// <summary>
        /// <see cref="Feature.Setup"/>
        /// </summary>
        protected override void Setup(FeatureConfigurationContext context)
        {
            Guard.AgainstNull(context, "context");

            ILogger logger;
            if (!context.Settings.TryGetSerilogTracingTarget(out logger))
            {
                logger = Log.Logger;
            }
            var logBuilder = new LogBuilder(logger, context.Settings.EndpointName());
            context.Container.ConfigureComponent(() => logBuilder, DependencyLifecycle.SingleInstance);


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