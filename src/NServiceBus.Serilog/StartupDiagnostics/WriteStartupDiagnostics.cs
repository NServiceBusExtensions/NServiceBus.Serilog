#region WriteStartupDiagnostics

class StartupDiagnostics :
    FeatureStartupTask
{
    public StartupDiagnostics(ReadOnlySettings settings, ILogger logger)
    {
        this.settings = settings;
        this.logger = logger.ForContext<StartupDiagnostics>();
    }

    protected override Task OnStart(IMessageSession session)
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
        ReadOnlySettings settings,
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
            return entry.Substring(12);
        }

        return entry;
    }

    protected override Task OnStop(IMessageSession session) =>
        Task.CompletedTask;

    ReadOnlySettings settings;
    ILogger logger;
}

#endregion