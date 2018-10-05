using System.Threading.Tasks;
using NServiceBus;
using Serilog;

public class TheHandler : IHandleMessages<StartHandler>
{
    static ILogger log = Log.ForContext<TheHandler>();

    public Task Handle(StartHandler message, IMessageHandlerContext context)
    {
        log.Information("Hello from {@Handler}. Message: {@Message}", nameof(TheHandler), message);
        IntegrationTests.resetEvent.Set();
        return Task.CompletedTask;
    }
}