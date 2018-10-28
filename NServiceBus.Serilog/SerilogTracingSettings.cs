﻿using Serilog;

namespace NServiceBus.Serilog
{
    /// <summary>
    /// All settings for Serilog Tracing.
    /// </summary>
    public class SerilogTracingSettings
    {
        internal ILogger Logger;
        EndpointConfiguration configuration;

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
        /// Enable tracing of messages. Measure the performance impact of this setting on the system.
        /// </summary>
        public void EnableMessageTracing()
        {
            configuration.EnableFeature<MessageTracingFeature>();
        }
    }
}