using NServiceBus;

public class StartHandler : IMessage
{
    public string Property { get; set; }
}