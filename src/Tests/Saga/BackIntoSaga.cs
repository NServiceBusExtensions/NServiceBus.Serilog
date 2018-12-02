using NServiceBus;

public class BackIntoSaga :
    IMessage
{
    public string Property { get; set; }
}