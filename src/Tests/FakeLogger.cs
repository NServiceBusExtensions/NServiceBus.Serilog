public class FakeLogger : ILogger
{
    public void Write(LogEvent logEvent) =>
        throw new NotImplementedException();

    public ILogger ForContext(IEnumerable<ILogEventEnricher> enrichers)
    {
        Enrichers = enrichers.ToList();
        return this;
    }

    public List<ILogEventEnricher>? Enrichers { get; set; }

    public ILogger ForContext(string propertyName, object? value, bool destructureObjects = false)
    {
        ContextKey = (string?)value;
        return this;
    }

    public string? ContextKey { get; set; }
}