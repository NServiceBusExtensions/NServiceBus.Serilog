using System.Threading.Tasks;
using NServiceBus;

public class TheHandler : IHandleMessages<StartHandler>
{
    public Task Handle(StartHandler message, IMessageHandlerContext context)
    {
        context.Logger().Information("Hello from {@Handler}.");
        IntegrationTests.resetEvent.Set();
        return Task.CompletedTask;
    }
}