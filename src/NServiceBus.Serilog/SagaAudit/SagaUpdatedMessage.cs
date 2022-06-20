namespace NServiceBus.Serilog;

class SagaUpdatedMessage
{
    public List<Dictionary<string, string>> ResultingMessages { get; } = new();
}