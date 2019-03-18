using NServiceBus;

public class UserCreated :
    IMessage
{
    public string UserName { get; set; }
    public string FamilyName { get; set; }
    public string GivenNames{ get; set; }
}