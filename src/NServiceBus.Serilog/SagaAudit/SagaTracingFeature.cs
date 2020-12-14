using NServiceBus;
using NServiceBus.Features;

class SagaTracingFeature :
    Feature
{
    public SagaTracingFeature()
    {
        DependsOn<Sagas>();
        DependsOn<TracingFeature>();
    }

    protected override void Setup(FeatureConfigurationContext context)
    {
        var settings = context.Settings.TracingSettings();
        var pipeline = context.Pipeline;
        pipeline.Register(new CaptureSagaStateBehavior.Registration(settings.useFullTypeName));
        pipeline.Register(new CaptureSagaResultingMessagesBehavior.Registration(settings.useFullTypeName));
    }
}