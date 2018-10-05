using System.Collections.Generic;
using System.Linq;
using Serilog.Events;

public static class TestExtensions
{
    public static LogEvent LogForType<T>(this IEnumerable<LogEvent> logs)
    {
        return LogsForType<T>(logs).SingleOrDefault();
    }

    public static IEnumerable<LogEvent> LogsForNsbSerilog<T>(this IEnumerable<LogEvent> logs)
    {
        foreach (var log in logs)
        {
            if (log.TryGetSourceContext(out var value))
            {
                if (value.StartsWith("NServiceBus.Serilog."))
                {
                    yield return log;
                }
            }
        }
    }

    public static IEnumerable<LogEvent> LogsForType<T>(this IEnumerable<LogEvent> logs)
    {
        return LogsForName(logs, typeof(T).Name);
    }

    public static IEnumerable<LogEvent> LogsForName(this IEnumerable<LogEvent> logs, string name)
    {
        foreach (var log in logs)
        {
            if (log.TryGetSourceContext(out var value))
            {
                if (value == name)
                {
                    yield return log;
                }
            }
        }
    }

    public static bool TryGetSourceContext(this LogEvent log, out string value)
    {
        if (log.Properties.TryGetValue("SourceContext", out var sourceContext))
        {
            if (sourceContext is ScalarValue scalarValue)
            {
                if (scalarValue.Value is string temp)
                {
                    value = temp;
                    return true;
                }
            }
        }

        value = null;
        return false;
    }
}