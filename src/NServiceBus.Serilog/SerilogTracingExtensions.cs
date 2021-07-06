using NServiceBus.Configuration.AdvancedExtensibility;
using NServiceBus.Serilog;
using NServiceBus.Settings;
using Serilog;
using Serilog.Configuration;

namespace NServiceBus
{
    /// <summary>
    /// Extensions to enable and configure Serilog Tracing.
    /// </summary>
    public static partial class SerilogTracingExtensions
    {
        /// <summary>
        /// Enable Serilog Tracing for this endpoint using <see cref="Log.Logger"/> as the logging target.
        /// </summary>
        public static SerilogTracingSettings EnableSerilogTracing(this EndpointConfiguration configuration)
        {
            return configuration.EnableSerilogTracing(Log.Logger);
        }

        public static LoggerEnrichmentConfiguration WithNsbExceptionDetails(this LoggerEnrichmentConfiguration configuration)
        {
            configuration.With<ExceptionEnricher>();
            return configuration;
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
            var serilogTracing = new SerilogTracingSettings(logger, configuration);
            settings.Set(serilogTracing);
            return serilogTracing;
        }

        internal static SerilogTracingSettings TracingSettings(this ReadOnlySettings settings)
        {
            return settings.Get<SerilogTracingSettings>();
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

            var type = context.GetType();
            while (true)
            {
                if (type.Name == "TestableMessageHandlerContext")
                {
                    context.Extensions.Set(Log.Logger);
                    return Log.Logger;
                }

                type = type.BaseType;
                if (type == null)
                {
                    break;
                }
            }

            throw new($@"Expected to find a `{nameof(ILogger)}` in the pipeline context.
It is possible NServiceBus.Serilog has not been enabled using a call to `{nameof(SerilogTracingExtensions)}.{nameof(EnableSerilogTracing)}()`.");
        }
    }
}