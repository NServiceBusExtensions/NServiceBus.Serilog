using System.Collections.Generic;
using System.Linq;
using Serilog.Events;

public static class TestExtensions
{
    public static IEnumerable<LogEventEx> LogsForNsbSerilog(this IEnumerable<LogEventEx> logs)
    {
        return logs.Where(log =>
            {
                var sourceContext = log.StringSourceContext();
                return sourceContext != null && sourceContext.StartsWith("NServiceBus.Serilog.");
            })
            .OrderBy(x => x.MessageTemplate.Text);
    }

    public static IEnumerable<LogEventEx> LogsWithExceptions(this IEnumerable<LogEventEx> logs)
    {
        return logs.Where(x=>x.Exception!=null);
    }

    public static IEnumerable<LogEventEx> LogsForType<T>(this IEnumerable<LogEventEx> logs)
    {
        return LogsForName(logs, typeof(T).Name)
            .OrderBy(x => x.MessageTemplate.Text);
    }

    public static IEnumerable<LogEventEx> LogsForName(this IEnumerable<LogEventEx> logs, string name)
    {
        return logs.Where(log => log.StringSourceContext() == name)
            .OrderBy(x => x.StringSourceContext());
    }

    public static string StringSourceContext(this LogEventEx log)
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