using NServiceBus;
using NServiceBus.Features;
using NServiceBus.Serilog;

class TracingLog : Feature
{
    protected override void Setup(FeatureConfigurationContext context)
    {
        var settings = context.Settings.Get<SerilogTracingSettings>();

        var logBuilder = new LogBuilder(settings.Logger, context.Settings.EndpointName());

        var pipeline = context.Pipeline;
        pipeline.Register(new ReceiveMessageBehavior.Registration(logBuilder));
        pipeline.Register(new SendMessageBehavior.Registration(logBuilder));
        if (settings.UseSagaTracing)
        {
            pipeline.Register(new CaptureSagaStateBehavior.Registration(logBuilder));
            pipeline.Register<CaptureSagaResultingMessagesBehavior.Registration>();
        }
    }
}