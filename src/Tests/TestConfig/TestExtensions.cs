public static class TestExtensions
{
    public static void DisableRetries(this EndpointConfiguration configuration)
    {
        var recoverability = configuration.Recoverability();
        recoverability.Delayed(_ => _.NumberOfRetries(0));
        recoverability.Immediate(_ => _.NumberOfRetries(0));
    }

    public static IEnumerable<LogEventEx> LogsForNsbSerilog(this IEnumerable<LogEventEx> logs) =>
        logs
            .Where(log =>
            {
                var context = log.StringSourceContext;
                return context != null && context.StartsWith("NServiceBus.Serilog.");
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
            .Where(_ => _.StringSourceContext == name)
            .OrderBy(_ => _.StringSourceContext);

    public static Task WriteLog()
    {
        Log.Error("LogMessage");
        return Task.CompletedTask;
    }
}