using System;
using NServiceBus.Logging;
using Serilog;
using Serilog.Events;

class Logger : ILog
{
    ILogger logger;

    public Logger(ILogger logger)
    {
        this.logger = logger;
    }

    public void Debug(string message)
    {
        logger.Debug("{Text:l}", message);
    }

    public void Debug(string message, Exception exception)
    {
        logger.Debug(exception, "{Text:l}", message);
    }

    public void DebugFormat(string format, params object[] args)
    {
        logger.Debug(format, args);
    }

    public void Info(string message)
    {
        logger.Information("{Text:l}", message);
    }

    public void Info(string message, Exception exception)
    {
        logger.Information(exception, "{Text:l}", message);
    }

    public void InfoFormat(string format, params object[] args)
    {
        logger.Information(format, args);
    }

    public void Warn(string message)
    {
        logger.Warning("{Text:l}", message);
    }

    public void Warn(string message, Exception exception)
    {
        logger.Warning(exception, "{Text:l}", message);
    }

    public void WarnFormat(string format, params object[] args)
    {
        logger.Warning(format, args);
    }

    public void Error(string message)
    {
        logger.Error("{Text:l}", message);
    }

    public void Error(string message, Exception exception)
    {
        logger.Error(exception, "{Text:l}", message);
    }

    public void ErrorFormat(string format, params object[] args)
    {
        logger.Error(format, args);
    }

    public void Fatal(string message)
    {
        logger.Fatal("{Text:l}", message);
    }

    public void Fatal(string message, Exception exception)
    {
        logger.Fatal(exception, "{Text:l}", message);
    }

    public void FatalFormat(string format, params object[] args)
    {
        logger.Fatal(format, args);
    }

    public bool IsDebugEnabled => logger.IsEnabled(LogEventLevel.Debug);
    public bool IsInfoEnabled => logger.IsEnabled(LogEventLevel.Information);
    public bool IsWarnEnabled => logger.IsEnabled(LogEventLevel.Warning);
    public bool IsErrorEnabled => logger.IsEnabled(LogEventLevel.Error);
    public bool IsFatalEnabled => logger.IsEnabled(LogEventLevel.Fatal);
}