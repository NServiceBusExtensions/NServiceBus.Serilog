using System.Collections.Generic;
using System.Linq;
using NServiceBus;
using NServiceBus.Serilog;

public static class TestExtensions
{
    public static void DisableRetries(this EndpointConfiguration configuration)
    {
        var recoverability = configuration.Recoverability();
        recoverability.Delayed(settings => { settings.NumberOfRetries(0); });
        recoverability.Immediate(settings => { settings.NumberOfRetries(0); });
    }

    public static IEnumerable<LogEventEx> LogsForNsbSerilog(this IEnumerable<LogEventEx> logs)
    {
        return logs.Where(log =>
            {
                var sourceContext = log.StringSourceContext;
                return sourceContext != null && sourceContext.StartsWith("NServiceBus.Serilog.");
            })
            .OrderBy(x => x.MessageTemplate.Text);
    }

    public static IEnumerable<LogEventEx> LogsWithExceptions(this IEnumerable<LogEventEx> logs)
    {
        return logs.Where(x => x.Exception != null);
    }

    public static IEnumerable<LogEventEx> LogsForType<T>(this IEnumerable<LogEventEx> logs)
    {
        return LogsForName(logs, TypeNameConverter.GetName(typeof(T)))
            .OrderBy(x => x.MessageTemplate.Text);
    }

    static IEnumerable<LogEventEx> LogsForName(this IEnumerable<LogEventEx> logs, string name)
    {
        return logs.Where(log => log.StringSourceContext == name)
            .OrderBy(x => x.StringSourceContext);
    }
}