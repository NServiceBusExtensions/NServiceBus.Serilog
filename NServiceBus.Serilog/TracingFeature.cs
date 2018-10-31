using NServiceBus;
using NServiceBus.Features;
using NServiceBus.Serilog;

class TracingFeature : Feature
{
    protected override void Setup(FeatureConfigurationContext context)
    {
        var settings = context.Settings.Get<SerilogTracingSettings>();

        var endpoint = context.Settings.EndpointName();
        var logBuilder = new LogBuilder(settings.Logger, endpoint);

        var pipeline = context.Pipeline;
        pipeline.Register(new InjectIncomingPhysicalMessageBehavior.Registration(logBuilder, endpoint));
        pipeline.Register(new InjectInvokeHandlerContextBehavior.Registration());
        pipeline.Register(new InjectOutgoingLogicalMessageBehavior.Registration(logBuilder));
    }
}