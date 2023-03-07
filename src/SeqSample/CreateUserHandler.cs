public class CreateUserHandler :
    IHandleMessages<CreateUser>
{
    public Task Handle(CreateUser message, HandlerContext context)
    {
        context.LogInformation("Hello from {@Handler}", nameof(CreateUserHandler));
        return Task.CompletedTask;
    }
}