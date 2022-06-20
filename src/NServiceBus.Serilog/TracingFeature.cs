class TracingFeature :
    Feature
{
    protected override void Setup(FeatureConfigurationContext context)
    {
        var settings = context.Settings.TracingSettings();

        var endpoint = context.Settings.EndpointName();
        var logBuilder = new LogBuilder(settings.Logger, endpoint);

        var pipeline = context.Pipeline;
        pipeline.Register(new InjectIncomingPhysicalBehavior.Registration(logBuilder, endpoint));
        pipeline.Register(new InjectIncomingLogicalBehavior.Registration(logBuilder));
        pipeline.Register(new InjectHandlerContextBehavior.Registration());
        pipeline.Register(new InjectMessageContextBehavior.Registration());
        pipeline.Register(new InjectOutgoingBehavior.Registration(logBuilder));
        context.RegisterStartupTask(new StartupDiagnostics(context.Settings, logBuilder.Logger));
    }
}