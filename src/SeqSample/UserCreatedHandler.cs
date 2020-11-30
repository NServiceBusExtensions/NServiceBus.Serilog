using System.Threading.Tasks;
using NServiceBus;

public class UserCreatedHandler :
    IHandleMessages<UserCreated>
{
    public Task Handle(UserCreated message, IMessageHandlerContext context)
    {
        context.Logger().Information("Hello from {@Handler}", nameof(UserCreatedHandler));
        return Task.CompletedTask;
    }
}