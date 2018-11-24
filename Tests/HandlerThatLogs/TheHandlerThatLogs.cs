using System;
using System.Threading.Tasks;
using NServiceBus;
using NServiceBus.Logging;
using Serilog;

public class TheHandlerThatLogs : IHandleMessages<StartHandlerThatLogs>
{
    public Task Handle(StartHandlerThatLogs message, IMessageHandlerContext context)
    {
        var logger = LogManager.GetLogger<TheHandlerThatThrows>();
        logger.Error("The message", new Exception());
        IntegrationTests.resetEvent.Set();
        return Task.CompletedTask;
    }
}