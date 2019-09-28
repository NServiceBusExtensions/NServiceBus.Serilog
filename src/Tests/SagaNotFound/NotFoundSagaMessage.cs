using NServiceBus;

public class NotFoundSagaMessage :
    IMessage
{
    public string? Property { get; set; }
}