using NServiceBus;
using NServiceBus.Features;

class TracingFeature :
    Feature
{
    protected override void Setup(FeatureConfigurationContext context)
    {
        var settings = context.Settings.TracingSettings();

        var endpoint = context.Settings.EndpointName();
        var logBuilder = new LogBuilder(settings.Logger, endpoint);

        var pipeline = context.Pipeline;
        pipeline.Register(new InjectIncomingMessageBehavior.Registration(logBuilder, endpoint));
        pipeline.Register(new InjectInvokeHandlerContextBehavior.Registration());
        pipeline.Register(new InjectOutgoingMessageBehavior.Registration(logBuilder));
        context.RegisterStartupTask(new WriteStartupDiagnostics(context.Settings,logBuilder.Logger));
    }
}