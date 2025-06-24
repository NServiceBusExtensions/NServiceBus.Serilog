class TracingFeature :
    Feature
{
    protected override void Setup(FeatureConfigurationContext context)
    {
        var settings = context.Settings.TracingSettings();

        var endpoint = context.Settings.EndpointName();
        var logBuilder = new LogBuilder(settings.Logger, endpoint);

        var pipeline = context.Pipeline;
        pipeline.Register(new IncomingPhysicalBehavior.Registration(endpoint));
        pipeline.Register(new IncomingLogicalBehavior.Registration());
        pipeline.Register(new HandlerContextBehavior.Registration());
        pipeline.Register(new MessageContextBehavior.Registration());
        pipeline.Register(new OutgoingBehavior.Registration());
        context.RegisterStartupTask(new StartupDiagnostics(context.Settings, logBuilder.Logger));
    }
}