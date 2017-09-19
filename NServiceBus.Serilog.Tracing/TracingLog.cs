using NServiceBus.Features;
using Serilog;

namespace NServiceBus.Serilog.Tracing
{
    /// <summary>
    /// Enabled tracing of messages to Serilog.
    /// </summary>
    public class TracingLog : Feature
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

            if (!context.Settings.TryGetSerilogTracingTarget(out var logger))
            {
                logger = Log.Logger;
            }
            //TODO: don't use container
            var logBuilder = new LogBuilder(logger, context.Settings.EndpointName());
            context.Container.ConfigureComponent(() => logBuilder, DependencyLifecycle.SingleInstance);

            var pipeline = context.Pipeline;
            pipeline.Register<ReceiveMessageBehavior.Registration>();
            pipeline.Register<SendMessageBehavior.Registration>();
        }

    }
}