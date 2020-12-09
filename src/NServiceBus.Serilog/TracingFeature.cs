using NServiceBus;
using NServiceBus.Features;

class TracingFeature :
    Feature
{
    protected override void Setup(FeatureConfigurationContext context)
    {
        var settings = context.Settings.TracingSettings();

        var endpoint = context.Settings.EndpointName();
        LogBuilder logBuilder = new(settings.Logger, endpoint);

        var pipeline = context.Pipeline;
        pipeline.Register(new InjectIncomingMessageBehavior.Registration(logBuilder, endpoint, settings.useFullTypeName));
        pipeline.Register(new InjectInvokeHandlerContextBehavior.Registration());
        pipeline.Register(new InjectOutgoingMessageBehavior.Registration(logBuilder, settings.useFullTypeName));
        context.RegisterStartupTask(new StartupDiagnostics(context.Settings, logBuilder.Logger));
    }
}