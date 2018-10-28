using NServiceBus.Features;

class MessageTracingFeature : Feature
{
    public MessageTracingFeature()
    {
        DependsOn<TracingFeature>();
    }

    protected override void Setup(FeatureConfigurationContext context)
    {
        var pipeline = context.Pipeline;
        pipeline.Register(new LogIncomingLogicalMessageBehavior.Registration());
        pipeline.Register(new LogOutgoingLogicalMessageBehavior.Registration());
    }
}