using Serilog;

namespace NServiceBus.Serilog
{
    /// <summary>
    /// All settings for Serilog Tracing.
    /// </summary>
    public class SerilogTracingSettings
    {
        internal ILogger Logger;
        internal bool UseSagaTracing;
        internal bool UseMessageTracing;

        internal SerilogTracingSettings(ILogger logger)
        {
            Logger = logger;
        }

        /// <summary>
        /// Enable tracing of saga state. Measure the performance impact of this setting on the system.
        /// </summary>
        public void EnableSagaTracing()
        {
            UseSagaTracing = true;
        }

        /// <summary>
        /// Enable tracing of messages. Measure the performance impact of this setting on the system.
        /// </summary>
        public void EnableMessageTracing()
        {
            UseMessageTracing = true;
        }
    }
}