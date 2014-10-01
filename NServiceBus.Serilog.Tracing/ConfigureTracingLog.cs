namespace NServiceBus.Serilog.Tracing
{
    using global::Serilog;
    using NServiceBus.Configuration.AdvanceExtensibility;
    using NServiceBus.Settings;

    public static class ConfigureTracingLog
    {

        /// <summary>
        ///   Defines a custom <see cref="ILogger"/> to use for by <see cref="TracingLog"/> to taregt. If not defined then <see cref="Log.Logger"/> will be used.
        /// </summary>
        public static void SerilogTracingTarget(this BusConfiguration busConfiguration, ILogger logger)
        {
            var settings = busConfiguration.GetSettings();
            settings.Set("customSerilogTracingTarget", logger);
        }

        internal static bool TryGetSerilogTracingTarget(this ReadOnlySettings settings,out ILogger logger)
        {
            return settings.TryGet("customSerilogTracingTarget", out logger);
        }

    }
}