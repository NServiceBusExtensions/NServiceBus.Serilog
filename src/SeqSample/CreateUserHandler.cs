public class CreateUserHandler :
    IHandleMessages<CreateUser>
{
    public Task Handle(CreateUser message, IMessageHandlerContext context)
    {
        context.LogInformation("Hello from {@Handler}", nameof(CreateUserHandler));
        return Task.CompletedTask;
    }
}