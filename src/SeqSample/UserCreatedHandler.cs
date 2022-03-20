public class UserCreatedHandler :
    IHandleMessages<UserCreated>
{
    public Task Handle(UserCreated message, IMessageHandlerContext context)
    {
        context.LogInformation("Hello from {@Handler}", nameof(UserCreatedHandler));
        return Task.CompletedTask;
    }
}