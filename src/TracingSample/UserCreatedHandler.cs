public class UserCreatedHandler :
    IHandleMessages<UserCreated>
{
    public Task Handle(UserCreated message, HandlerContext context)
    {
        Log.Information("Hello from UserCreatedHandler");
        throw new("The error");
    }
}