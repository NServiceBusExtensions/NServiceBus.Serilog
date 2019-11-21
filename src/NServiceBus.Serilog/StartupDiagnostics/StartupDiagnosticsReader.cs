using System;
using System.Collections.Generic;
using System.Reflection;
using NServiceBus;
using NServiceBus.Settings;

static class StartupDiagnosticsReader
{
    public static List<StartupDiagnosticEntries.StartupDiagnosticEntry> ReadStartupDiagnosticEntries(this ReadOnlySettings readOnlySettings)
    {
        var diagnosticEntries = readOnlySettings.Get<StartupDiagnosticEntries>();
        var field = diagnosticEntries.GetType().GetField("entries", BindingFlags.Instance | BindingFlags.NonPublic);
        if (field == null)
        {
            throw new Exception($"Could not extract 'entries' field from {nameof(StartupDiagnosticEntries)}.");
        }

        return (List<StartupDiagnosticEntries.StartupDiagnosticEntry>) field.GetValue(diagnosticEntries);
    }
}