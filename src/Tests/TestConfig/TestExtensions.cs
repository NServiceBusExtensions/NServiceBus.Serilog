﻿public static class TestExtensions
{
    public static void DisableRetries(this EndpointConfiguration configuration)
    {
        var recoverability = configuration.Recoverability();
        recoverability.Delayed(settings =>
        {
            settings.NumberOfRetries(0);
        });
        recoverability.Immediate(settings =>
        {
            settings.NumberOfRetries(0);
        });
    }

    public static IEnumerable<LogEventEx> LogsForNsbSerilog(this IEnumerable<LogEventEx> logs) =>
        logs
            .Where(log =>
            {
                var sourceContext = log.StringSourceContext;
                return sourceContext != null && sourceContext.StartsWith("NServiceBus.Serilog.");
            })
            .OrderBy(_ => _.MessageTemplate.Text);

    public static IEnumerable<LogEventEx> LogsWithExceptions(this IEnumerable<LogEventEx> logs) =>
        logs.Where(_ => _.Exception is not null);

    public static IEnumerable<LogEventEx> LogsForType<T>(this IEnumerable<LogEventEx> logs) =>
        LogsForName(logs, TypeNameConverter.GetName(typeof(T))
                .MessageTypeName)
            .OrderBy(_ => _.MessageTemplate.Text);

    static IEnumerable<LogEventEx> LogsForName(this IEnumerable<LogEventEx> logs, string name) =>
        logs
            .Where(log => log.StringSourceContext == name)
            .OrderBy(_ => _.StringSourceContext);
}