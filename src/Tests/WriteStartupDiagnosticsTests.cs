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
        return Verify(settings.ReadStartupDiagnosticEntries());
    }

    [Fact]
    public void CleanEntry()
    {
        Assert.Equal("Persistence.Sql.SqlDialect", StartupDiagnostics.CleanEntry("NServiceBus.Persistence.Sql.SqlDialect"));
        Assert.Equal("Transport.SqlServer.CircuitBreaker", StartupDiagnostics.CleanEntry("NServiceBus.Transport.SqlServer.CircuitBreaker"));
        Assert.Equal("Foo", StartupDiagnostics.CleanEntry("Foo"));
    }
}