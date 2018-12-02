using System.Threading.Tasks;
using NServiceBus;

public class CreateUserHandler : IHandleMessages<CreateUser>
{
    public Task Handle(CreateUser message, IMessageHandlerContext context)
    {
        var logger = context.Logger();
        logger.Information("Hello from {@Handler}.");
        return Task.CompletedTask;
    }
}