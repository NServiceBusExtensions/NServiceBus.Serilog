﻿using NServiceBus;
using NServiceBus.Features;

class MessageTracingFeature :
    Feature
{
    public MessageTracingFeature()
    {
        DependsOn<TracingFeature>();
    }

    protected override void Setup(FeatureConfigurationContext context)
    {
        var settings = context.Settings.TracingSettings();
        var pipeline = context.Pipeline;
        pipeline.Register(new LogIncomingBehavior.Registration(settings.useFullTypeName));
        pipeline.Register(new LogOutgoingBehavior.Registration(settings.useFullTypeName));
    }
}