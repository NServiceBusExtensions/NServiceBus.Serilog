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
            ILogger logger;
            if (!context.Settings.TryGetSerilogTracingTarget(out logger))
            {
                logger = Log.Logger;
            }
            var logBuilder = new LogBuilder(logger, context.Settings.EndpointName());
            context.Container.ConfigureComponent(() => logBuilder, DependencyLifecycle.SingleInstance);
        }
    }
}