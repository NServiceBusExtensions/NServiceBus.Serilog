using Serilog;

namespace NServiceBus.Serilog
{
    /// <summary>
    /// All settings for Serilog Tracing.
    /// </summary>
    public class SerilogTracingSettings
    {
        internal ILogger Logger;
        EndpointConfiguration configuration;
        internal bool useFullTypeName;
        internal ConvertHeader convertHeader = (_, _) => null;

        internal SerilogTracingSettings(ILogger logger, EndpointConfiguration configuration)
        {
            Logger = logger;
            this.configuration = configuration;
        }

        /// <summary>
        /// Enable tracing of saga state. Measure the performance impact of this setting on the system.
        /// </summary>
        public void EnableSagaTracing()
        {
            configuration.EnableFeature<SagaTracingFeature>();
        }

        /// <summary>
        /// Use fully assembly qualified name for types.
        /// </summary>
        public void UseFullTypeName()
        {
            useFullTypeName = true;
        }

        /// <summary>
        /// Allow a custom log property to be used to a specific header
        /// </summary>
        public void UseHeaderConversion(ConvertHeader convertHeader)
        {
            Guard.AgainstNull(convertHeader, nameof(convertHeader));
            this.convertHeader = convertHeader;
        }

        /// <summary>
        /// Enable tracing of messages. Measure the performance impact of this setting on the system.
        /// </summary>
        public void EnableMessageTracing()
        {
            configuration.EnableFeature<MessageTracingFeature>();
        }
    }
}