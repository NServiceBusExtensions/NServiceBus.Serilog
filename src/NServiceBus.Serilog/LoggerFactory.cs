﻿class LoggerFactory(ILogger logger) :
    ILoggerFactory
{
    public ILog GetLogger(Type type)
    {
        var contextLogger = logger.ForContext(type);
        return new Logger(contextLogger);
    }

    public ILog GetLogger(string name)
    {
        var contextLogger = logger.ForContext("SourceContext", name);
        return new Logger(contextLogger);
    }
}