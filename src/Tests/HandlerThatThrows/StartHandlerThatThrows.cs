using NServiceBus;

public class StartHandlerThatThrows : IMessage
{
    public string Property { get; set; }
}