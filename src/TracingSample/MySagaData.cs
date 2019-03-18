using NServiceBus;

public class MySagaData :
    ContainSagaData
{
    public string UserName { get; set; }
}