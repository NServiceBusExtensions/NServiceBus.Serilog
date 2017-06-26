using System;
using System.Collections.Generic;
using Serilog;
using Serilog.Events;

static class SerilogExtensions
{
    public static LogEventProperty BindProperty(this ILogger logger, string name, object value)
    {
        logger.BindProperty(name, value, true, out LogEventProperty property);
        return property;
    }

    public static void WriteInfo(this ILogger logger, MessageTemplate messageTemplate, IEnumerable<LogEventProperty> properties)
    {
        var logEvent = new LogEvent(
            timestamp: DateTimeOffset.Now,
            level: LogEventLevel.Information,
            exception: null,
            messageTemplate: messageTemplate,
            properties: properties);
        logger.Write(logEvent);
    }
}