#region WriteStartupDiagnostics

class StartupDiagnostics :
    FeatureStartupTask
{
    public StartupDiagnostics(IReadOnlySettings settings, ILogger logger)
    {
        this.settings = settings;
        this.logger = logger.ForContext<StartupDiagnostics>();
    }

    protected override Task OnStart(IMessageSession session, Cancel cancel = default)
    {
        var properties = BuildProperties(settings, logger);

        var templateParser = new MessageTemplateParser();
        var messageTemplate = templateParser.Parse("DiagnosticEntries");
        var logEvent = new LogEvent(
            timestamp: DateTimeOffset.Now,
            level: LogEventLevel.Warning,
            exception: null,
            messageTemplate: messageTemplate,
            properties: properties);
        logger.Write(logEvent);
        return Task.CompletedTask;
    }

    static IEnumerable<LogEventProperty> BuildProperties(
        IReadOnlySettings settings,
        ILogger logger)
    {
        var entries = settings.ReadStartupDiagnosticEntries();
        foreach (var entry in entries)
        {
            if (entry.Name == "Features")
            {
                continue;
            }

            var name = CleanEntry(entry.Name);
            if (logger.BindProperty(name, entry.Data, out var property))
            {
                yield return property;
            }
        }
    }

    internal static string CleanEntry(string entry)
    {
        if (entry.StartsWith("NServiceBus."))
        {
            return entry[12..];
        }

        return entry;
    }

    protected override Task OnStop(IMessageSession session, Cancel cancel = default) =>
        Task.CompletedTask;

    IReadOnlySettings settings;
    ILogger logger;
}

#endregion