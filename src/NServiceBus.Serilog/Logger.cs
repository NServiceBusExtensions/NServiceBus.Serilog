using System;
using System.Collections.Generic;
using NServiceBus.Logging;
using NServiceBus.Serilog;
using Serilog;
using Serilog.Events;
using Serilog.Parsing;

class Logger : ILog
{
    ILogger logger;
    MessageTemplateParser templateParser;

    public Logger(ILogger logger)
    {
        this.logger = logger;
        templateParser = new MessageTemplateParser();
    }

    void WriteExceptionEvent(string message, Exception exception, LogEventLevel level)
    {
        var data = exception.Data;
        if (!data.Contains("ExceptionLogState"))
        {
            logger.Write(level, exception, message);
            return;
        }
        var logState = (ExceptionLogState)data["ExceptionLogState"];
        data.Remove("ExceptionLogState");
        var properties = new List<LogEventProperty>
        {
            new LogEventProperty("ProcessingEndpoint", new ScalarValue(logState.ProcessingEndpoint)),
            new LogEventProperty("MessageId", new ScalarValue(logState.MessageId)),
            new LogEventProperty("MessageType", new ScalarValue(logState.MessageType))
        };
        if (logState.CorrelationId != null)
        {
            properties.Add(new LogEventProperty("CorrelationId", new ScalarValue(logState.CorrelationId)));
        }

        if (logState.ConversationId != null)
        {
            properties.Add(new LogEventProperty("ConversationId", new ScalarValue(logState.ConversationId)));
        }

        var messageTemplate = templateParser.Parse(message);
        var logEvent = new LogEvent(DateTimeOffset.Now, level, exception, messageTemplate, properties);
        logger.Write(logEvent);
    }

    public void Debug(string message)
    {
        logger.Debug(message);
    }

    public void Debug(string message, Exception exception)
    {
        WriteExceptionEvent(message, exception, LogEventLevel.Debug);
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
        WriteExceptionEvent(message, exception, LogEventLevel.Information);
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
        WriteExceptionEvent(message, exception, LogEventLevel.Warning);
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
        WriteExceptionEvent(message, exception, LogEventLevel.Error);
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
        WriteExceptionEvent(message, exception, LogEventLevel.Fatal);
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