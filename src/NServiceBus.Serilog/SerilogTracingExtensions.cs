namespace NServiceBus;

/// <summary>
/// Extensions to enable and configure Serilog Tracing.
/// </summary>
public static partial class SerilogTracingExtensions
{
    /// <summary>
    /// Enable Serilog Tracing for this endpoint using Log.Logger as the logging target.
    /// </summary>
    public static SerilogTracingSettings EnableSerilogTracing(this EndpointConfiguration configuration) =>
        configuration.EnableSerilogTracing(Log.Logger);

    /// <summary>
    /// Take NSB specific info from <see cref="Exception.Data" /> and promotes it to Serilog properties.
    /// </summary>
    public static LoggerEnrichmentConfiguration WithNsbExceptionDetails(this LoggerEnrichmentConfiguration configuration, ConvertHeader? convertHeader = null)
    {
        configuration.With(new ExceptionEnricher(convertHeader));
        return configuration;
    }

    /// <summary>
    /// Enable Serilog Tracing for this endpoint.
    /// </summary>
    public static SerilogTracingSettings EnableSerilogTracing(this EndpointConfiguration configuration, ILogger logger)
    {
        configuration.EnableFeature<TracingFeature>();
        var settings = configuration.GetSettings();
        var serilogTracing = new SerilogTracingSettings(logger, configuration);
        settings.Set(serilogTracing);
        return serilogTracing;
    }

    internal static SerilogTracingSettings TracingSettings(this IReadOnlySettings settings) =>
        settings.Get<SerilogTracingSettings>();
}