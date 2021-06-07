using NServiceBus;

public class StartBehaviorThatThrows :
    IMessage
{
    public string? Property { get; set; }
}