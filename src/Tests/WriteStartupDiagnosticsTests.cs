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
        SettingsHolder settings = new();
        StartupDiagnosticEntries diagnosticEntries = new();
        diagnosticEntries.Add("Name", "Value");
        settings.Set(diagnosticEntries);
        return Verifier.Verify(settings.ReadStartupDiagnosticEntries());
    }

    [Fact]
    public void CleanEntry()
    {
        Assert.Equal("Persistence.Sql.SqlDialect", StartupDiagnostics.CleanEntry("NServiceBus.Persistence.Sql.SqlDialect"));
        Assert.Equal("Transport.SqlServer.CircuitBreaker", StartupDiagnostics.CleanEntry("NServiceBus.Transport.SqlServer.CircuitBreaker"));
        Assert.Equal("Foo", StartupDiagnostics.CleanEntry("Foo"));
    }
}