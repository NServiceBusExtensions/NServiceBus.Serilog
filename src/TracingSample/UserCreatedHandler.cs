public class UserCreatedHandler :
    IHandleMessages<UserCreated>
{
    public Task Handle(UserCreated message, IMessageHandlerContext context)
    {
        context.LogInformation("Hello from UserCreatedHandler");
        throw new("The error");
    }
}