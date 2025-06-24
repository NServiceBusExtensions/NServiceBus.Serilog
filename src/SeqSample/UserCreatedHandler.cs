public class UserCreatedHandler :
    IHandleMessages<UserCreated>
{
    public Task Handle(UserCreated message, HandlerContext context)
    {
        Log.Information("Hello from {@Handler}", nameof(UserCreatedHandler));
        return Task.CompletedTask;
    }
}