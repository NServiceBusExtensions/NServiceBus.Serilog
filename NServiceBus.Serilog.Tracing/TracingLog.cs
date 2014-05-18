using System;
using Serilog;
using Serilog.Core;

namespace NServiceBus.Serilog.Tracing
{
    public static class TracingLog
    {
        static bool initialized;
        static ILogger logger;

        public static void Enable(ILogger logger)
        {
            VerifyNotInitialized();
            initialized = true;
            TracingLog.logger = logger;
        }

        public static void Disable()
        {
            VerifyNotInitialized();
            initialized = true;
        }

        internal static ILogger GetLogger(string sourceContext)
        {
            VerifyInitialized();

            return logger
                .ForContext(Constants.SourceContextPropertyName, sourceContext);
        }

        static void VerifyInitialized()
        {
            if (!initialized)
            {
                throw new Exception("Must Call either TracingLog.Enable or TracingLog.Disable before starting the bus.");
            }
        }

        internal static bool IsEnabled()
        {
            VerifyInitialized();
            return logger != null;
        }
        static void VerifyNotInitialized()
        {
            if (initialized)
            {
                throw new Exception("Enable or Disable can only be called once.");
            }
        }

    }
}