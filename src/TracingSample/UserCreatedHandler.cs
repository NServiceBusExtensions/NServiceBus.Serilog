public class UserCreatedHandler :
    IHandleMessages<UserCreated>
{
    public Task Handle(UserCreated message, HandlerContext context)
    {
        context.LogInformation("Hello from UserCreatedHandler");
        throw new("The error");
    }
}