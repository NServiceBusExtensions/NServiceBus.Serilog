using System;
using System.Collections.Generic;
using NServiceBus.Logging;
using Serilog;
using Serilog.Events;
using Serilog.Parsing;

class Logger : ILog
{
    ILogger logger;

    public Logger(ILogger logger)
    {
        this.logger = logger;
    }
    void WriteException(string message, Exception exception, LogEventLevel level)
    {
        if (!exception.Data.Contains("ExceptionLogState"))
        {
            logger.Write(level, message);
            return;
        }

        var exceptionLogState = (ExceptionLogState)exception.Data["ExceptionLogState"];
        exception.Data.Remove("ExceptionLogState");
        var properties = new List<LogEventProperty>
        {
            new LogEventProperty("ProcessingEndpoint", new ScalarValue(exceptionLogState.Endpoint)),
            new LogEventProperty("MessageId", new ScalarValue(exceptionLogState.MessageId)),
            new LogEventProperty("MessageType", new ScalarValue(exceptionLogState.MessageType))
        };
        if (exceptionLogState.CorrelationId != null)
        {
            properties.Add(new LogEventProperty("CorrelationId", new ScalarValue(exceptionLogState.CorrelationId)));
        }

        if (exceptionLogState.ConversationId != null)
        {
            properties.Add(new LogEventProperty("ConversationId", new ScalarValue(exceptionLogState.ConversationId)));
        }

        var messageTemplate = new MessageTemplate(message, new TextToken[] { });
        var logEvent = new LogEvent(DateTimeOffset.Now, level, exception, messageTemplate, properties);
        logger.Write(logEvent);
    }

    public void Debug(string message)
    {
        logger.Debug(message);
    }

    public void Debug(string message, Exception exception)
    {
        WriteException(message, exception, LogEventLevel.Debug);
    }

    public void DebugFormat(string format, params object[] args)
    {
        logger.Debug(format, args);
    }

    public void Info(string message)
    {
        logger.Information(message);
    }

    public void Info(string message, Exception exception)
    {
        WriteException(message, exception, LogEventLevel.Information);
    }

    public void InfoFormat(string format, params object[] args)
    {
        logger.Information(format, args);
    }

    public void Warn(string message)
    {
        logger.Warning(message);
    }

    public void Warn(string message, Exception exception)
    {
        WriteException(message, exception, LogEventLevel.Warning);
    }

    public void WarnFormat(string format, params object[] args)
    {
        logger.Warning(format, args);
    }

    public void Error(string message)
    {
        logger.Error(message);
    }

    public void Error(string message, Exception exception)
    {
        WriteException(message, exception, LogEventLevel.Error);
    }

    public void ErrorFormat(string format, params object[] args)
    {
        logger.Error(format, args);
    }

    public void Fatal(string message)
    {
        logger.Fatal(message);
    }

    public void Fatal(string message, Exception exception)
    {
        WriteException(message, exception, LogEventLevel.Fatal);
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