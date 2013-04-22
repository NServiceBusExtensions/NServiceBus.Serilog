using System;
using NServiceBus.Logging;
using Serilog;

namespace NServiceBusSerilog
{
    class LoggerFactory : ILoggerFactory
    {
        ILogger logger;

        public LoggerFactory(ILogger logger)
        {
            this.logger = logger;
        }

        public ILog GetLogger(Type type)
        {
            return GetLogger(type.FullName);
        }

        public ILog GetLogger(string name)
        {
            var contextLogger = logger.ForContext("SourceContext", name);
            return new Logger(contextLogger);
        }
    }
}