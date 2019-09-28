using System.Threading;
using System.Threading.Tasks;
using NServiceBus;

public class TheHandler :
    IHandleMessages<StartHandler>
{
    ManualResetEvent resetEvent;

    public TheHandler(ManualResetEvent resetEvent)
    {
        this.resetEvent = resetEvent;
    }

    public Task Handle(StartHandler message, IMessageHandlerContext context)
    {
        var logger = context.Logger();
        logger.Information("Hello from {@Handler}.");
        resetEvent.Set();
        return Task.CompletedTask;
    }
}