public class CreateUserHandler :
    IHandleMessages<CreateUser>
{
    public Task Handle(CreateUser message, HandlerContext context)
    {
        Log.Information("Hello from {@Handler}.", nameof(CreateUserHandler));
        return Task.FromResult(0);
    }
}