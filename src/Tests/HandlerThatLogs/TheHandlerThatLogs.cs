using System;
using System.Threading;
using System.Threading.Tasks;
using NServiceBus;
using NServiceBus.Logging;

public class TheHandlerThatLogs :
    IHandleMessages<StartHandlerThatLogs>
{
    ManualResetEvent resetEvent;

    public TheHandlerThatLogs(ManualResetEvent resetEvent)
    {
        this.resetEvent = resetEvent;
    }

    public Task Handle(StartHandlerThatLogs message, IMessageHandlerContext context)
    {
        var logger = LogManager.GetLogger<TheHandlerThatThrows>();
        logger.Error("The message", new Exception());
        resetEvent.Set();
        return Task.CompletedTask;
    }
}