using System.Collections.Generic;
using System.Linq;
using Serilog.Events;

public static class TestExtensions
{
    public static LogEvent LogForType<T>(this IEnumerable<LogEvent> logs)
    {
        return LogsForType<T>(logs).SingleOrDefault();
    }

    public static IEnumerable<LogEvent> LogsForNsbSerilog(this IEnumerable<LogEvent> logs)
    {
        return logs.Where(log =>
            {
                var sourceContext = log.StringSourceContext();
                return sourceContext != null && sourceContext.StartsWith("NServiceBus.Serilog.");
            })
            .OrderBy(x => x.StringSourceContext());
    }

    public static IEnumerable<LogEvent> LogsForType<T>(this IEnumerable<LogEvent> logs)
    {
        return LogsForName(logs, typeof(T).Name)
            .OrderBy(x=>x.StringSourceContext());
    }

    public static IEnumerable<LogEvent> LogsForName(this IEnumerable<LogEvent> logs, string name)
    {
        return logs.Where(log => log.StringSourceContext() == name)
            .OrderBy(x => x.StringSourceContext());
    }

    public static string StringSourceContext(this LogEvent log)
    {
        if (log.Properties.TryGetValue("SourceContext", out var sourceContext))
        {
            if (sourceContext is ScalarValue scalarValue)
            {
                if (scalarValue.Value is string temp)
                {
                    return temp;
                }
            }
        }

        return null;
    }
}