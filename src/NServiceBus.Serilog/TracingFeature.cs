﻿using NServiceBus;
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
        pipeline.Register(new InjectIncomingBehavior.Registration(logBuilder, endpoint, settings.useFullTypeName));
        pipeline.Register(new InjectHandlerContextBehavior.Registration());
        pipeline.Register(new InjectOutgoingBehavior.Registration(logBuilder, settings.useFullTypeName));
        context.RegisterStartupTask(new StartupDiagnostics(context.Settings, logBuilder.Logger));
    }
}