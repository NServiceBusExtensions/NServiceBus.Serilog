using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using NServiceBus;
using NServiceBus.Features;
using NServiceBus.Settings;
using Serilog;
using Serilog.Events;
using Serilog.Parsing;

#region WriteStartupDiagnostics
class WriteStartupDiagnostics :
    FeatureStartupTask
{
    public WriteStartupDiagnostics(ReadOnlySettings settings, ILogger logger)
    {
        this.settings = settings;
        this.logger = logger;
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

    static IEnumerable<LogEventProperty> BuildProperties(ReadOnlySettings readOnlySettings, ILogger logger)
    {
        var entries = readOnlySettings.ReadStartupDiagnosticEntries();
        foreach (var entry in entries)
        {
            if (entry.Name == "Features")
            {
                continue;
            }
            if (logger.BindProperty(entry.Name, entry.Data, out var property))
            {
                yield return property!;
            }
        }
    }

    protected override Task OnStop(IMessageSession session)
    {
        return Task.CompletedTask;
    }

    ReadOnlySettings settings;
    private readonly ILogger logger;
}
#endregion