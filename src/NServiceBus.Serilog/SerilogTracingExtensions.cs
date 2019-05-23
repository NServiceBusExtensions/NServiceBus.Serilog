using NServiceBus.Configuration.AdvancedExtensibility;
using NServiceBus.Serilog;
using Serilog;

namespace NServiceBus
{
    /// <summary>
    /// Extensions to enable and configure Serilog Tracing.
    /// </summary>
    public static class SerilogTracingExtensions
    {
        /// <summary>
        /// Enable Serilog Tracing for this endpoint using <see cref="Log.Logger"/> as the logging target.
        /// </summary>
        public static SerilogTracingSettings EnableSerilogTracing(this EndpointConfiguration configuration)
        {
            return configuration.EnableSerilogTracing(Log.Logger);
        }

        /// <summary>
        /// Enable Serilog Tracing for this endpoint.
        /// </summary>
        public static SerilogTracingSettings EnableSerilogTracing(this EndpointConfiguration configuration, ILogger logger)
        {
            Guard.AgainstNull(configuration, nameof(configuration));
            Guard.AgainstNull(logger, nameof(logger));
            var recoverability = configuration.Recoverability();
            recoverability.AddUnrecoverableException<ConfigurationException>();
            configuration.EnableFeature<TracingFeature>();
            var settings = configuration.GetSettings();
            var attachments = new SerilogTracingSettings(logger,configuration);
            settings.Set(attachments);
            return attachments;
        }

        /// <summary>
        /// Get the current <see cref="ILogger"/> for this context.
        /// </summary>
        public static ILogger Logger(this IPipelineContext context)
        {
            Guard.AgainstNull(context, nameof(context));
            var bag = context.Extensions;
            if (bag.TryGet("SerilogOutgoingLogger", out ILogger logger))
            {
                return logger;
            }
            if (bag.TryGet("SerilogHandlerLogger", out logger))
            {
                return logger;
            }
            if (bag.TryGet(out logger))
            {
                return logger;
            }
            throw new ConfigurationException($@"Expected to find a {nameof(ILogger)} in the pipeline context.
 * If this is a production exception, it is possible NServiceBus.Serilog has not been enabled using a call to {nameof(SerilogTracingExtensions)}.{nameof(EnableSerilogTracing)}().
 * If this is a unit test exception, it is possible an {nameof(ILogger)} has not been injected into the test context. eg context.Extensions.Set(Log.Logger);.");
        }
    }
}