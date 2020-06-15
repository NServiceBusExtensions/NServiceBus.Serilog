using System.Threading.Tasks;
using NServiceBus;
using NServiceBus.Settings;
using VerifyXunit;
using Xunit;

[UsesVerify]
public class WriteStartupDiagnosticsTests
{
    [Fact]
    public Task Can_extract_settings()
    {
        var settings = new SettingsHolder();
        var diagnosticEntries = new StartupDiagnosticEntries();
        diagnosticEntries.Add("Name", "Value");
        settings.Set(diagnosticEntries);
        return Verifier.Verify(settings.ReadStartupDiagnosticEntries());
    }
}