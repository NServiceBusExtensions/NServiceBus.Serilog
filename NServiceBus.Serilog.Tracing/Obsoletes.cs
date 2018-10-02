using System;
using NServiceBus.Features;
using Serilog;
// ReSharper disable UnusedParameter.Global

#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member

namespace NServiceBus.Serilog.Tracing
{
    static class Obsoletes
    {
        public const string Message = @"Use endpointConfiguration.EnableSerilogTracing(tracingLog) instead.
To enable with saga tracing use:
var serilogTracing = endpointConfiguration.EnableSerilogTracing(tracingLog);
serilogTracing.EnableSagaTracing();
";
    }

    [Obsolete(Obsoletes.Message,true)]
    public class TracingLog : Feature
    {
        protected override void Setup(FeatureConfigurationContext context)
        {
            throw new NotImplementedException();
        }
    }

    [Obsolete(Obsoletes.Message,true)]
    public class SagaTracing : Feature
    {
        protected override void Setup(FeatureConfigurationContext context)
        {
            throw new NotImplementedException();
        }
    }

    [Obsolete(Obsoletes.Message,true)]
    public static class ConfigureTracingLog
    {
        [Obsolete(Obsoletes.Message,true)]
        public static void SerilogTracingTarget(this EndpointConfiguration configuration, ILogger logger)
        {
        }
    }
}