namespace NServiceBus.Serilog;

class SagaUpdatedMessage
{
    public SagaUpdatedMessage(DateTimeOffset startTime) =>
        StartTime = startTime;

    public Guid SagaId { get; set; }
    public List<Dictionary<string, string>> ResultingMessages { get; } = new();
    public bool IsNew { get; set; }
    public bool IsCompleted { get; set; }
    public DateTimeOffset StartTime { get; }
    public DateTimeOffset FinishTime { get; set; }
    public string? SagaType { get; set; }
}