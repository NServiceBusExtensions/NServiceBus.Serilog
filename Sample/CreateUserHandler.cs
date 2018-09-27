using System.Threading.Tasks;
using NServiceBus;
using Serilog;

public class CreateUserHandler : IHandleMessages<CreateUser>
{
    static ILogger log = Log.ForContext<CreateUserHandler>();

    public Task Handle(CreateUser message, IMessageHandlerContext context)
    {
        log.Information("Hello from {@Handler}. Message: {@Message}", nameof(CreateUserHandler), message);
        return Task.CompletedTask;
    }
}