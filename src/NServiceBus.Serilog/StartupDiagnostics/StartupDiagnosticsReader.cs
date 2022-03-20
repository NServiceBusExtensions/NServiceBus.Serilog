static class StartupDiagnosticsReader
{
    public static List<StartupDiagnosticEntries.StartupDiagnosticEntry> ReadStartupDiagnosticEntries(this ReadOnlySettings readOnlySettings)
    {
        var diagnosticEntries = readOnlySettings.Get<StartupDiagnosticEntries>();
        var field = diagnosticEntries.GetType().GetField("entries", BindingFlags.Instance | BindingFlags.NonPublic);
        if (field is null)
        {
            throw new($"Could not extract 'entries' field from {nameof(StartupDiagnosticEntries)}.");
        }

        return (List<StartupDiagnosticEntries.StartupDiagnosticEntry>) field.GetValue(diagnosticEntries)!;
    }
}