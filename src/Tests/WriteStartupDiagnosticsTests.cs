using NServiceBus;
using NServiceBus.Settings;
using Xunit;
using Xunit.Abstractions;

public class WriteStartupDiagnosticsTests :
    XunitApprovalBase
{
    [Fact]
    public void Can_extract_settings()
    {
        var settings = new SettingsHolder();
        var diagnosticEntries = new StartupDiagnosticEntries();
        diagnosticEntries.Add("Name", "Value");
        settings.Set(diagnosticEntries);
        ObjectApprover.Verify(settings.ReadStartupDiagnosticEntries());
    }

    public WriteStartupDiagnosticsTests(ITestOutputHelper output) :
        base(output)
    {
    }
}