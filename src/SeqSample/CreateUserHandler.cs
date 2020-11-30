using System.Threading.Tasks;
using NServiceBus;

public class CreateUserHandler :
    IHandleMessages<CreateUser>
{
    public Task Handle(CreateUser message, IMessageHandlerContext context)
    {
        context.Logger().Information("Hello from {@Handler}", nameof(CreateUserHandler));
        return Task.CompletedTask;
    }
}