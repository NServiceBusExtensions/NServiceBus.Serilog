using NServiceBus.Logging;
using Serilog;

namespace NServiceBusSerilog
{
    public static class SerilogConfigurator
    {
        public static void Configure()
        {
            Configure(Log.Logger);
        }

        public static void Configure(ILogger logger)
        {
            LogManager.LoggerFactory = new LoggerFactory(logger);
        }
    }
}