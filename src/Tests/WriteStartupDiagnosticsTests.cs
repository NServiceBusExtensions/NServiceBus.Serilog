using System.Threading.Tasks;
using NServiceBus;
using NServiceBus.Settings;
using VerifyXunit;
using Xunit;
using Xunit.Abstractions;

public class WriteStartupDiagnosticsTests :
    VerifyBase
{
    [Fact]
    public Task Can_extract_settings()
    {
        var settings = new SettingsHolder();
        var diagnosticEntries = new StartupDiagnosticEntries();
        diagnosticEntries.Add("Name", "Value");
        settings.Set(diagnosticEntries);
        return Verify(settings.ReadStartupDiagnosticEntries());
    }

    public WriteStartupDiagnosticsTests(ITestOutputHelper output) :
        base(output)
    {
    }
}