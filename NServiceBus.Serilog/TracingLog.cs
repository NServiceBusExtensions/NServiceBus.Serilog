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
        pipeline.Register(new IncomingPhysicalMessageBehavior.Registration(logBuilder));
        pipeline.Register(new IncomingLogicalMessageBehavior.Registration());
        pipeline.Register(new InvokeHandlerContextBehavior.Registration());
        pipeline.Register(new OutgoingLogicalMessageBehavior.Registration(logBuilder));
        if (settings.UseSagaTracing)
        {
            pipeline.Register(new CaptureSagaStateBehavior.Registration());
            pipeline.Register<CaptureSagaResultingMessagesBehavior.Registration>();
        }
    }
}