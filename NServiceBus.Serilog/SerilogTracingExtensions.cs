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
            configuration.EnableFeature<TracingLog>();
            var settings = configuration.GetSettings();
            var attachments = new SerilogTracingSettings(logger);
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
            if (bag.TryGet("SerilogHandlerLogger", out ILogger logger))
            {
                return logger;
            }
            return bag.Get<ILogger>();
        }
    }
}