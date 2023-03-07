public class UserCreatedHandler :
    IHandleMessages<UserCreated>
{
    public Task Handle(UserCreated message, HandlerContext context)
    {
        context.LogInformation("Hello from {@Handler}", nameof(UserCreatedHandler));
        return Task.CompletedTask;
    }
}