using NServiceBus;
using NServiceBus.Features;
using NServiceBus.Serilog;

class TracingFeature : Feature
{
    protected override void Setup(FeatureConfigurationContext context)
    {
        var settings = context.Settings.Get<SerilogTracingSettings>();

        var logBuilder = new LogBuilder(settings.Logger, context.Settings.EndpointName());

        var pipeline = context.Pipeline;
        pipeline.Register(new InjectIncomingPhysicalMessageBehavior.Registration(logBuilder));
        pipeline.Register(new InjectInvokeHandlerContextBehavior.Registration());
        pipeline.Register(new InjectOutgoingLogicalMessageBehavior.Registration(logBuilder));
    }
}