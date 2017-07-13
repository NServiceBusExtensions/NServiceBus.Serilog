using NServiceBus.Configuration.AdvancedExtensibility;

namespace NServiceBus.Serilog.Tracing
{
    using global::Serilog;
    using Settings;

    /// <summary>
    /// Provides extensions to <see cref="EndpointConfiguration"/> to configure serilog tracing.
    /// </summary>
    public static class ConfigureTracingLog
    {

        /// <summary>
        ///   Defines a custom <see cref="ILogger"/> to use for by <see cref="TracingLog"/> to target. If not defined then <see cref="Log.Logger"/> will be used.
        /// </summary>
        public static void SerilogTracingTarget(this EndpointConfiguration configuration, ILogger logger)
        {
            Guard.AgainstNull(configuration, "configuration");
            Guard.AgainstNull(logger, "logger");
            var settings = configuration.GetSettings();
            settings.Set("customSerilogTracingTarget", logger);
        }

        internal static bool TryGetSerilogTracingTarget(this ReadOnlySettings settings,out ILogger logger)
        {
            return settings.TryGet("customSerilogTracingTarget", out logger);
        }

    }
}